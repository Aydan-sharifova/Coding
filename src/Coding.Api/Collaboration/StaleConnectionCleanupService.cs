using Microsoft.AspNetCore.SignalR;

namespace Coding.Api.Collaboration;

public sealed class StaleConnectionCleanupService(
    ICollaborationPresenceTracker presence,
    IHubContext<CollaborationHub, ICollaborationClient> hub,
    ILogger<StaleConnectionCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan StaleAfter = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(SweepInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var removed = presence.RemoveStale(DateTime.UtcNow - StaleAfter);
            foreach (var connection in removed)
            {
                foreach (var projectId in connection.ProjectIds)
                {
                    if (presence.GetProjectConnectionCount(projectId, connection.UserId) == 0)
                        await hub.Clients.Group(CollaborationHub.ProjectGroup(projectId))
                            .UserLeft(new UserPresence(
                                projectId,
                                new CollaborationUser(
                                    connection.UserId,
                                    connection.UserName,
                                    connection.DisplayName,
                                    connection.AvatarUrl,
                                    0,
                                    connection.LastHeartbeat)));
                    await hub.Clients.Group(CollaborationHub.ProjectGroup(projectId))
                        .PresenceUpdated(new PresenceUpdate(projectId, presence.GetProjectUsers(projectId)));
                }
                logger.LogWarning(
                    "Removed stale collaboration connection {ConnectionId} for user {UserId}",
                    connection.ConnectionId, connection.UserId);
            }
        }
    }
}
