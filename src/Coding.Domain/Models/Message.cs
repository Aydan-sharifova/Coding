using System;
namespace Coding.Models
{
    public class Message:Base
    {
        public Guid WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public Guid SenderId { get; set; }

        public User Sender { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsEdited { get; set; }

        public bool IsDeleted { get; set; }
    }
}

