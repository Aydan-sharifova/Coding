using Coding.Enums;

namespace Coding.DTOS.Invitation
{
    public class InvitationGetDTO
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public InvitationStatus Status { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
