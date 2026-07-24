namespace Coding.Models;

public sealed class TaskComment : Base
{
    public Guid TaskId { get; set; }
    public ProjectTask Task { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
