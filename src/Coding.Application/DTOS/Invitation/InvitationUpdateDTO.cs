using Coding.Enums;

namespace Coding.DTOS.Invitation
{
    public class InvitationUpdateDTO
    {
        public Guid? WorkspaceId { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public InvitationStatus? Status { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
