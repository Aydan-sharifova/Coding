using Coding.Enums;

namespace Coding.DTOS.ProjectMember
{
    public class ProjectMemberUpdateDTO
    {
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public ProjectRole? Role { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
