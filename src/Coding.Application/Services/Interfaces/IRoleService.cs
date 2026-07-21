using Coding.DTOS.Auth;

namespace Coding.Services.Interfaces;

public interface IRoleService
{
    Task AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken);
}
