using Coding.Enums;

namespace Coding.Models;

public sealed class UserNotificationPreference : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool InAppEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; }
}
