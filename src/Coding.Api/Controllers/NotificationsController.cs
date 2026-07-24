using Coding.Application.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api/notifications")]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<NotificationPage> List([FromQuery] string? cursor, [FromQuery] int limit = 30, [FromQuery] bool? isRead = null, CancellationToken ct = default) => sender.Send(new GetNotificationsQuery(cursor, limit, isRead), ct);
    [HttpPut("{notificationId:guid}/read")]
    public async Task<IActionResult> Read(Guid notificationId, CancellationToken ct) { await sender.Send(new MarkNotificationReadCommand(notificationId), ct); return NoContent(); }
    [HttpPut("read-all")]
    public async Task<IActionResult> ReadAll(CancellationToken ct) { await sender.Send(new MarkAllNotificationsReadCommand(), ct); return NoContent(); }
}
