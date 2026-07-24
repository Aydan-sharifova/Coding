namespace Coding.Models;

public sealed class TaskAssignee
{
    public Guid TaskId { get; set; }
    public ProjectTask Task { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid AssignedByUserId { get; set; }
    public DateTime AssignedAt { get; set; }
}
