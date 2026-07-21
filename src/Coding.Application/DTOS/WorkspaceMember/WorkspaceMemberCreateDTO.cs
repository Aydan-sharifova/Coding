using Coding.Enums;

namespace Coding.DTOS.WorkspaceMember
{
    public class WorkspaceMemberCreateDTO
    {
        public Guid WorkspaceId { get; set; }
        public Guid UserId { get; set; }
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
