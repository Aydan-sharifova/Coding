using Coding.Enums;

namespace Coding.Models;

public class AccountToken : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AccountTokenType Type { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }
}
