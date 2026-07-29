using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class DemoLoginRequest
{
    [Required]
    [RegularExpression(
        "^(Owner|Admin|Member)$",
        ErrorMessage = "Role must be Owner, Admin, or Member.")]
    public string Role { get; init; } = string.Empty;
}
