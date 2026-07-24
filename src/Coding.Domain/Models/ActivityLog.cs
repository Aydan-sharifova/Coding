using System.Text.Json;

namespace Coding.Models;

public sealed class ActivityLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}
