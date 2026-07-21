using Coding.Enums;

namespace Coding.DTOS.Notification
{
    public class NotificationUpdateDTO
    {
        public Guid? UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType? Type { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
