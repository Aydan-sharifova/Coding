using Coding.Enums;

namespace Coding.DTOS.WorkspaceMember
{
    public class WorkspaceMemberGetDTO
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid UserId { get; set; }
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
