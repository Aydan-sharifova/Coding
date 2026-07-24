using Coding.Enums;

namespace Coding.Models;

public sealed class Conversation : Base
{
    public ConversationType Type { get; set; }
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    public string? Name { get; set; }
    public string? DirectKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ConversationParticipant> Participants { get; set; } = [];
    public ICollection<ChatMessage> ChatMessages { get; set; } = [];
}
