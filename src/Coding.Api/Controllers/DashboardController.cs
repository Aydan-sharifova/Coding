using Coding.Application.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api/dashboard")]
public sealed class DashboardController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<DashboardDto> Get(CancellationToken ct) => sender.Send(new GetDashboardQuery(), ct);
}
