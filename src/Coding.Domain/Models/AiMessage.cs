using Coding.Enums;

namespace Coding.Models;

public sealed class AiMessage : Base
{
    public Guid ConversationId { get; set; }
    public AiConversation Conversation { get; set; } = null!;
    public AiMessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public AiAssistantAction? Action { get; set; }
    public Guid? FileId { get; set; }
    public DateTime CreatedAt { get; set; }
}
