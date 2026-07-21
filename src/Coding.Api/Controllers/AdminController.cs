using Coding.DTOS.Auth;
using Coding.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IRoleService roleService;

    public AdminController(IRoleService roleService)
    {
        this.roleService = roleService;
    }

    [HttpPost("roles")]
    public async Task<IActionResult> AssignRole(
        AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        await roleService.AssignRoleAsync(request, cancellationToken);
        return NoContent();
    }
}
