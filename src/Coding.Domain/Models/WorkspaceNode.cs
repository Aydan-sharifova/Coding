using Coding.Enums;

namespace Coding.Models;

public sealed class WorkspaceNode : Base
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public WorkspaceNode? Parent { get; set; }
    public ICollection<WorkspaceNode> Children { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public WorkspaceNodeType NodeType { get; set; }
    public FileContent? FileContent { get; set; }
    public ICollection<FileVersion> Versions { get; set; } = [];
}
