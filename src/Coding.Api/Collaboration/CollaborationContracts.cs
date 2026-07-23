namespace Coding.Api.Collaboration;

public sealed record TextRange(
    int StartLineNumber,
    int StartColumn,
    int EndLineNumber,
    int EndColumn);

public sealed record CodeOperation(
    Guid OperationId,
    Guid FileId,
    Guid UserId,
    long ClientVersion,
    long BaseVersion,
    TextRange Range,
    string InsertedText,
    int DeletedLength,
    DateTime Timestamp);

public sealed record CursorPosition(
    Guid FileId,
    int LineNumber,
    int Column,
    TextRange? Selection);

public sealed record CollaborationUser(
    Guid UserId,
    string UserName,
    string DisplayName,
    string? AvatarUrl,
    int ConnectionCount,
    DateTime LastSeenAt);

public sealed record PresenceUpdate(Guid ProjectId, IReadOnlyCollection<CollaborationUser> Users);
public sealed record UserPresence(Guid ProjectId, CollaborationUser User);
public sealed record FileChangedMessage(Guid FileId, Guid ChangedByUserId, int VersionNumber, string ConcurrencyToken);
public sealed record ResyncRequiredMessage(Guid FileId, long ServerVersion, string Reason);
public sealed record OperationAcceptedMessage(Guid OperationId, Guid FileId, long ServerVersion);

public interface ICollaborationClient
{
    Task UserJoined(UserPresence message);
    Task UserLeft(UserPresence message);
    Task PresenceUpdated(PresenceUpdate message);
    Task CodeOperationReceived(CodeOperation operation);
    Task OperationAccepted(OperationAcceptedMessage message);
    Task ResyncRequired(ResyncRequiredMessage message);
    Task CursorUpdated(Guid userId, CursorPosition position);
    Task TypingStarted(Guid fileId, Guid userId);
    Task TypingStopped(Guid fileId, Guid userId);
    Task FileChanged(FileChangedMessage message);
}
