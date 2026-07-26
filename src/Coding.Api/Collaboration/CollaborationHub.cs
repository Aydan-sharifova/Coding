using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Coding.Data;
using Coding.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace Coding.Api.Collaboration;

[Authorize, EnableRateLimiting("realtime")]
public sealed class CollaborationHub(
    AppDbContext db,
    ICollaborationPresenceTracker presence,
    ILogger<CollaborationHub> logger) : Hub<ICollaborationClient>
{
    private static readonly ConcurrentDictionary<Guid, long> LiveVersions = new();
    private static readonly ConcurrentDictionary<string, long> LastCursorTicks = new();
    private static readonly TimeSpan CursorInterval = TimeSpan.FromMilliseconds(50);

    private Guid UserId => Guid.TryParse(Context.User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
        ? id
        : throw new HubException("The authenticated user identifier is invalid.");

    public override async Task OnConnectedAsync()
    {
        var userId = UserId;
        var user = await db.Users.AsNoTracking()
            .Where(item => item.ID == userId && !item.IsDeleted)
            .Select(item => new { item.UserName, item.FirstName, item.LastName, item.AvatarUrl })
            .SingleOrDefaultAsync(Context.ConnectionAborted)
            ?? throw new HubException("Authenticated user no longer exists.");

        presence.Connect(
            Context.ConnectionId,
            userId,
            user.UserName,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.AvatarUrl);
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        logger.LogInformation(
            "Collaboration connection {ConnectionId} established for user {UserId}",
            Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connection = presence.Disconnect(Context.ConnectionId);
        if (connection is not null)
        {
            foreach (var projectId in connection.ProjectIds)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ProjectGroup(projectId));
                if (presence.GetProjectConnectionCount(projectId, connection.UserId) == 0)
                    await Clients.Group(ProjectGroup(projectId)).UserLeft(
                        new UserPresence(projectId, ToUser(connection, 0)));
                await BroadcastPresence(projectId);
            }
        }

        LastCursorTicks.TryRemove(Context.ConnectionId, out _);
        logger.LogInformation(
            "Collaboration connection {ConnectionId} closed for user {UserId}. Error: {Error}",
            Context.ConnectionId, connection?.UserId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinProject(Guid projectId)
    {
        await RequireProjectMember(projectId);
        var wasAlreadyPresent = presence.GetProjectConnectionCount(projectId, UserId) > 0;
        presence.JoinProject(Context.ConnectionId, projectId);
        await Groups.AddToGroupAsync(Context.ConnectionId, ProjectGroup(projectId));

        var current = presence.GetProjectUsers(projectId).Single(user => user.UserId == UserId);
        if (!wasAlreadyPresent)
            await Clients.OthersInGroup(ProjectGroup(projectId))
                .UserJoined(new UserPresence(projectId, current));
        await BroadcastPresence(projectId);
    }

    public async Task LeaveProject(Guid projectId)
    {
        if (!presence.IsInProject(Context.ConnectionId, projectId))
            return;

        var current = presence.GetProjectUsers(projectId)
            .SingleOrDefault(user => user.UserId == UserId);
        presence.LeaveProject(Context.ConnectionId, projectId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ProjectGroup(projectId));
        if (presence.GetProjectConnectionCount(projectId, UserId) == 0)
            await Clients.Group(ProjectGroup(projectId))
                .UserLeft(new UserPresence(projectId, current is null
                    ? new CollaborationUser(UserId, "user", "User", null, 0, DateTime.UtcNow)
                    : current with { ConnectionCount = 0 }));
        await BroadcastPresence(projectId);
    }

    public async Task JoinFile(Guid fileId)
    {
        var projectId = await RequireFileMember(fileId);
        if (!presence.IsInProject(Context.ConnectionId, projectId))
            throw new HubException("Join the project before joining a file.");

        presence.JoinFile(Context.ConnectionId, fileId);
        await Groups.AddToGroupAsync(Context.ConnectionId, FileGroup(fileId));
        await InitializeLiveVersion(fileId);
    }

    public async Task LeaveFile(Guid fileId)
    {
        presence.LeaveFile(Context.ConnectionId, fileId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, FileGroup(fileId));
        await Clients.OthersInGroup(FileGroup(fileId)).TypingStopped(fileId, UserId);
    }

    public async Task<long> SendCodeOperation(CodeOperation operation)
    {
        if (operation.FileId == Guid.Empty || operation.OperationId == Guid.Empty)
            throw new HubException("A valid operation and file identifier are required.");
        if (operation.UserId != Guid.Empty && operation.UserId != UserId)
            throw new HubException("The operation user does not match the authenticated user.");
        if (!presence.IsInFile(Context.ConnectionId, operation.FileId))
            throw new HubException("Join the file before sending operations.");

        await RequireFileMember(operation.FileId);
        var serverVersion = await InitializeLiveVersion(operation.FileId);
        if (operation.BaseVersion != serverVersion)
        {
            await Clients.Caller.ResyncRequired(new ResyncRequiredMessage(
                operation.FileId, serverVersion, "The local base version is stale."));
            return -1;
        }

        var nextVersion = LiveVersions.AddOrUpdate(
            operation.FileId,
            _ => serverVersion + 1,
            (_, current) => current == serverVersion ? current + 1 : current);
        if (nextVersion != serverVersion + 1)
        {
            await Clients.Caller.ResyncRequired(new ResyncRequiredMessage(
                operation.FileId, nextVersion, "Another operation won the version race."));
            return -1;
        }

        var trustedOperation = operation with
        {
            UserId = UserId,
            ClientVersion = nextVersion,
            Timestamp = DateTime.UtcNow
        };
        await Clients.OthersInGroup(FileGroup(operation.FileId))
            .CodeOperationReceived(trustedOperation);
        await Clients.Caller.OperationAccepted(
            new OperationAcceptedMessage(operation.OperationId, operation.FileId, nextVersion));
        return nextVersion;
    }

    public async Task UpdateCursor(CursorPosition position)
    {
        if (!presence.IsInFile(Context.ConnectionId, position.FileId))
            throw new HubException("Join the file before updating a cursor.");
        if (position.LineNumber < 1 || position.Column < 1)
            throw new HubException("Cursor coordinates must be positive.");

        var now = DateTime.UtcNow.Ticks;
        var previous = LastCursorTicks.GetOrAdd(Context.ConnectionId, 0);
        if (new TimeSpan(now - previous) < CursorInterval)
            return;
        LastCursorTicks[Context.ConnectionId] = now;
        await Clients.OthersInGroup(FileGroup(position.FileId)).CursorUpdated(UserId, position);
    }

    public Task StartTyping(Guid fileId) => BroadcastTyping(fileId, true);
    public Task StopTyping(Guid fileId) => BroadcastTyping(fileId, false);

    public Task Heartbeat()
    {
        presence.Heartbeat(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public async Task JoinConversation(Guid conversationId)
    {
        if (!await db.ConversationParticipants.AsNoTracking().AnyAsync(item => item.ConversationId == conversationId && item.UserId == UserId))
            throw new HubException("You are not a participant in this conversation.");
        await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    public Task LeaveConversation(Guid conversationId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));

    public async Task StartChatTyping(Guid conversationId)
    {
        if (!await db.ConversationParticipants.AsNoTracking().AnyAsync(item => item.ConversationId == conversationId && item.UserId == UserId))
            throw new HubException("You are not a participant in this conversation.");
        await Clients.OthersInGroup(ConversationGroup(conversationId)).ChatTypingStarted(conversationId, UserId);
    }

    public async Task StopChatTyping(Guid conversationId)
    {
        if (!await db.ConversationParticipants.AsNoTracking().AnyAsync(item => item.ConversationId == conversationId && item.UserId == UserId))
            return;
        await Clients.OthersInGroup(ConversationGroup(conversationId)).ChatTypingStopped(conversationId, UserId);
    }

    public async Task NotifyFileChanged(Guid fileId, int versionNumber, string concurrencyToken)
    {
        await RequireFileMember(fileId);
        if (!presence.IsInFile(Context.ConnectionId, fileId))
            throw new HubException("Join the file before publishing a file change.");
        LiveVersions[fileId] = versionNumber;
        await Clients.OthersInGroup(FileGroup(fileId))
            .FileChanged(new FileChangedMessage(fileId, UserId, versionNumber, concurrencyToken));
    }

    private async Task BroadcastTyping(Guid fileId, bool started)
    {
        if (!presence.IsInFile(Context.ConnectionId, fileId))
            throw new HubException("Join the file before sending typing state.");
        await RequireFileMember(fileId);
        if (started)
            await Clients.OthersInGroup(FileGroup(fileId)).TypingStarted(fileId, UserId);
        else
            await Clients.OthersInGroup(FileGroup(fileId)).TypingStopped(fileId, UserId);
    }

    private async Task RequireProjectMember(Guid projectId)
    {
        var allowed = await db.ProjectMembers.AsNoTracking()
            .AnyAsync(member => member.ProjectId == projectId && member.UserId == UserId);
        if (!allowed)
            throw new HubException("You are not an active member of this project.");
    }

    private async Task<Guid> RequireFileMember(Guid fileId)
    {
        var projectId = await db.WorkspaceNodes.AsNoTracking()
            .Where(node => node.ID == fileId && node.NodeType == WorkspaceNodeType.File)
            .Select(node => (Guid?)node.ProjectId)
            .SingleOrDefaultAsync();
        if (projectId is null)
            throw new HubException("File not found.");
        await RequireProjectMember(projectId.Value);
        return projectId.Value;
    }

    private async Task<long> InitializeLiveVersion(Guid fileId)
    {
        if (LiveVersions.TryGetValue(fileId, out var version))
            return version;
        var persisted = await db.FileContents.AsNoTracking()
            .Where(content => content.NodeId == fileId)
            .Select(content => content.VersionNumber)
            .SingleOrDefaultAsync();
        return LiveVersions.GetOrAdd(fileId, persisted);
    }

    private Task BroadcastPresence(Guid projectId) =>
        Clients.Group(ProjectGroup(projectId))
            .PresenceUpdated(new PresenceUpdate(projectId, presence.GetProjectUsers(projectId)));

    private static CollaborationUser ToUser(ConnectionPresence connection, int connectionCount) =>
        new(connection.UserId, connection.UserName, connection.DisplayName, connection.AvatarUrl, connectionCount, connection.LastHeartbeat);

    public static string ProjectGroup(Guid projectId) => $"project:{projectId:N}";
    public static string FileGroup(Guid fileId) => $"file:{fileId:N}";
    public static string ConversationGroup(Guid conversationId) => $"conversation:{conversationId:N}";
    public static string UserGroup(Guid userId) => $"user:{userId:N}";
}
