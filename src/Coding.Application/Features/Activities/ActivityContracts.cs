using System.Text.Json;
using MediatR;

namespace Coding.Application.Features.Activities;

public sealed record ActivityWrite(Guid? UserId, Guid? ProjectId, string ActionType, string EntityType, Guid? EntityId, string Description, IReadOnlyDictionary<string, object?>? Metadata = null);
public interface IActivityLogger { Task LogAsync(ActivityWrite activity, CancellationToken cancellationToken = default); }
public sealed record ActivityLogDto(Guid Id, Guid? UserId, string? UserName, Guid? ProjectId, string? ProjectName, string ActionType, string EntityType, Guid? EntityId, string Description, JsonElement Metadata, string? IpAddress, string? UserAgent, DateTime CreatedAt);
public sealed record ActivityPage(IReadOnlyList<ActivityLogDto> Items, int Total, int Page, int PageSize);
public sealed record GetActivityLogsQuery(Guid? UserId, Guid? ProjectId, string? ActionType, string? EntityType, DateTime? From, DateTime? To, int Page = 1, int PageSize = 50) : IRequest<ActivityPage>;
