using Coding.Application.Features.Analytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api/analytics")]
public sealed class AnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<AnalyticsDashboardDto> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? projectId, CancellationToken ct) =>
        sender.Send(new GetAnalyticsDashboardQuery(from, to, projectId), ct);

    [HttpPost("coding-sessions")]
    public async Task<ActionResult<object>> Start(StartCodingSessionCommand command, CancellationToken ct) =>
        Ok(new { sessionId = await sender.Send(command, ct) });

    [HttpPost("coding-sessions/{sessionId:guid}/heartbeat")]
    public async Task<IActionResult> Heartbeat(Guid sessionId, CancellationToken ct)
    {
        await sender.Send(new HeartbeatCodingSessionCommand(sessionId), ct);
        return NoContent();
    }

    [HttpPost("coding-sessions/{sessionId:guid}/end")]
    public async Task<IActionResult> End(Guid sessionId, CancellationToken ct)
    {
        await sender.Send(new EndCodingSessionCommand(sessionId), ct);
        return NoContent();
    }
}
