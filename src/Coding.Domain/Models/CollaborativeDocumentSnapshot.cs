namespace Coding.Models;

public sealed class CollaborativeDocumentSnapshot
{
    public Guid ID { get; set; }
    public Guid ProjectId { get; set; }
    public Guid FileId { get; set; }
    public byte[] EncodedState { get; set; } = [];
    public byte[] StateVector { get; set; } = [];
    public long SequenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string ContentHash { get; set; } = string.Empty;
    public bool IsCompacted { get; set; }
}
