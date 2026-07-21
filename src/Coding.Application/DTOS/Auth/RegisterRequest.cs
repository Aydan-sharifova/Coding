using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class RegisterRequest
{
    [Required, MaxLength(50)]
    public string FirstName { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string UserName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(12), MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}
