namespace Coding.Models;

public sealed class AiUsageRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid ConversationId { get; set; }
    public AiConversation Conversation { get; set; } = null!;
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int DurationMs { get; set; }
    public bool WasCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
}
