namespace Coding.Models;

public sealed class CodingSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid FileId { get; set; }
    public WorkspaceNode File { get; set; } = null!;
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
