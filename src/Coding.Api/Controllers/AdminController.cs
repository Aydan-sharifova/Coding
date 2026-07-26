using Coding.Application.Features.Administration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Route("api/admin"), Authorize(Roles = "SuperAdmin,Admin")]
public sealed class AdminController(ISender sender) : ControllerBase
{
    [HttpGet("statistics")] public Task<PlatformStatistics> Statistics(CancellationToken ct) => sender.Send(new GetPlatformStatisticsQuery(), ct);
    [HttpGet("users")] public Task<PageResult<AdminUserListItem>> Users([FromQuery] string? search, [FromQuery] bool? suspended, [FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default) => sender.Send(new GetAdminUsersQuery(search, suspended, role, page, pageSize), ct);
    [HttpGet("users/{userId:guid}")] public Task<AdminUserDetails> UserDetails(Guid userId, CancellationToken ct) => sender.Send(new GetAdminUserDetailsQuery(userId), ct);
    [HttpPut("users/{userId:guid}/suspension")] public async Task<IActionResult> Suspension(Guid userId, SetSuspensionRequest request, CancellationToken ct) { await sender.Send(new SetUserSuspensionCommand(userId, request.Suspended, request.Reason), ct); return NoContent(); }
    [HttpPut("users/{userId:guid}/roles/{role}"), Authorize(Roles = "SuperAdmin")] public async Task<IActionResult> Role(Guid userId, string role, SetRoleRequest request, CancellationToken ct) { await sender.Send(new SetSystemRoleCommand(userId, role, request.Enabled), ct); return NoContent(); }
    [HttpPut("users/{userId:guid}"), Authorize(Roles = "SuperAdmin")] public Task<AdminUserDetails> UpdateUser(Guid userId, UpdateUserRequest request, CancellationToken ct) => sender.Send(new UpdateAdminUserCommand(userId, request.FirstName, request.LastName, request.UserName, request.Email, request.Bio), ct);
    [HttpDelete("users/{userId:guid}"), Authorize(Roles = "SuperAdmin")] public async Task<IActionResult> DeleteUser(Guid userId, [FromBody] DeleteUserRequest request, CancellationToken ct) { await sender.Send(new DeleteAdminUserCommand(userId, request.Reason), ct); return NoContent(); }
    [HttpGet("projects")] public Task<PageResult<AdminProjectItem>> Projects([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default) => sender.Send(new GetAdminProjectsQuery(search, page, pageSize), ct);
    [HttpDelete("projects/{projectId:guid}")] public async Task<IActionResult> DeleteProject(Guid projectId, [FromBody] DeleteProjectRequest request, CancellationToken ct) { await sender.Send(new DeleteAbusiveProjectCommand(projectId, request.Reason), ct); return NoContent(); }
}
public sealed record SetSuspensionRequest(bool Suspended, string? Reason);
public sealed record SetRoleRequest(bool Enabled);
public sealed record DeleteProjectRequest(string Reason);
public sealed record UpdateUserRequest(string FirstName, string LastName, string UserName, string Email, string? Bio);
public sealed record DeleteUserRequest(string Reason);
