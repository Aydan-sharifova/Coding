using Coding.Enums;

namespace Coding.DTOS.Invitation
{
    public class InvitationCreateDTO
    {
        public Guid WorkspaceId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public InvitationStatus Status { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
