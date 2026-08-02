namespace Coding.Application.Features.Collaboration;

public sealed record CollaborativeSnapshotData(byte[] EncodedState, byte[] StateVector, long SequenceNumber, string ContentHash);
public sealed record CollaborativeUpdateData(Guid ProjectId, Guid FileId, Guid UpdateId, byte[] EncodedUpdate, long SequenceNumber, DateTime CreatedAt, Guid CreatedByUserId);

public interface ICollaborativeDocumentStore
{
    Task<CollaborativeSnapshotData?> GetLatestSnapshotAsync(Guid fileId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CollaborativeUpdateData>> GetUpdatesAfterAsync(Guid fileId, long sequenceNumber, CancellationToken cancellationToken);
    Task<(bool Appended, long SequenceNumber)> AppendUpdateAsync(Guid projectId, Guid fileId, Guid updateId, byte[] update, Guid userId, CancellationToken cancellationToken);
    Task SaveSnapshotAsync(Guid projectId, Guid fileId, byte[] state, byte[] stateVector, long sequenceNumber, Guid userId, CancellationToken cancellationToken);
    Task CompactDocumentAsync(Guid projectId, Guid fileId, byte[] mergedState, byte[] stateVector, long throughSequence, Guid userId, CancellationToken cancellationToken);
}

public interface ICollaborativeContentMaterializer
{
    void Enqueue(Guid projectId, Guid fileId, Guid userId, string content);
}
