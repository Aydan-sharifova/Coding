using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class ResetPasswordRequest
{
    [Required]
    public string Token { get; init; } = string.Empty;

    [Required, MinLength(12), MaxLength(128)]
    public string NewPassword { get; init; } = string.Empty;
}
