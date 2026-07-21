using System;
using Coding.Enums;

namespace Coding.Models
{
    public class Notification:Base
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

