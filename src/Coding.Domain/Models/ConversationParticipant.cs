namespace Coding.Models;

public sealed class ConversationParticipant : Base
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
    public Guid? LastReadMessageId { get; set; }
}
