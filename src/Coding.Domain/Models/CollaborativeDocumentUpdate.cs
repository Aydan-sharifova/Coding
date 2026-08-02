namespace Coding.Models;

public sealed class CollaborativeDocumentUpdate
{
    public Guid ID { get; set; }
    public Guid ProjectId { get; set; }
    public Guid FileId { get; set; }
    public Guid UpdateId { get; set; }
    public byte[] EncodedUpdate { get; set; } = [];
    public long SequenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
}
