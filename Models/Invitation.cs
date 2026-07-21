using System;
using Coding.Enums;

namespace Coding.Models
{
    public class Invitation:Base
    {
        public Guid WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public InvitationStatus Status { get; set; }

        public DateTime ExpireDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

