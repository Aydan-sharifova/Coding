using System;
namespace Coding.Models
{
    public class User : Base
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public bool IsOnline { get; set; }

        public DateTime LastSeen { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public ICollection<WorkspaceMember> WorkspaceMembers { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }

        public ICollection<Message> Messages { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<CodeHistory> CodeHistories { get; set; }
    }
}

