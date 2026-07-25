namespace Coding.Models;

public sealed class UserPreference
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Theme { get; set; } = "system";
    public string Language { get; set; } = "en";
    public bool ReducedMotion { get; set; }
    public bool CompactMode { get; set; }
    public bool SecurityAlertsEnabled { get; set; } = true;
    public DateTime UpdatedAt { get; set; }
}
