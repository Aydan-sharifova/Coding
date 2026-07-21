using System;
namespace Coding.Models
{
    public class Project:Base
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid? WorkspaceId { get; set; }

        public Workspace? Workspace { get; set; }

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public string DefaultLanguage { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectMember> Members { get; set; } = [];

        public ICollection<ProjectInvitation> Invitations { get; set; } = [];

        public ICollection<Folder> Folders { get; set; } = [];

        public ICollection<GitCommit> Commits { get; set; } = [];
    }
}
