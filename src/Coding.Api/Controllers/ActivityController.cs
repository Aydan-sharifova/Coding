using Coding.Application.Features.Activities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize(Roles = "Admin"), Route("api/admin/activities")]
public sealed class ActivityController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<ActivityPage> List([FromQuery] Guid? userId, [FromQuery] Guid? projectId, [FromQuery] string? actionType, [FromQuery] string? entityType, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default) => sender.Send(new GetActivityLogsQuery(userId, projectId, actionType, entityType, from, to, page, pageSize), ct);
}
