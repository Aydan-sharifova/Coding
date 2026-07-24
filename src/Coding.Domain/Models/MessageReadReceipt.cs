namespace Coding.Models;

public sealed class MessageReadReceipt
{
    public Guid MessageId { get; set; }
    public ChatMessage Message { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ReadAt { get; set; }
}
