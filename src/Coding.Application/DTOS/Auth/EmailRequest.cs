using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class EmailRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;
}
