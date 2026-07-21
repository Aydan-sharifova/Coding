using System;
using Coding.Enums;

namespace Coding.Models
{
    public class ProjectMember:Base
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public ProjectRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
