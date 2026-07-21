using System;
using Coding.Models;

namespace Coding.Models
{
    public class CodeHistory:Base
    {

        public Guid FileItemId { get; set; }

        public FileItem FileItem { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public string OldContent { get; set; }

        public string NewContent { get; set; }

        public DateTime EditedAt { get; set; }
    }
}

