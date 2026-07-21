using System.ComponentModel.DataAnnotations;

namespace Coding.DTOS.Auth;

public sealed class AssignRoleRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public string Role { get; init; } = string.Empty;
}
