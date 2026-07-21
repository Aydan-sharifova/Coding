using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}
