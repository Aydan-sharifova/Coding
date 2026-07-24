using MediatR;

namespace Coding.Application.Features.Dashboard;

public sealed record DashboardMetricDto(string Key, string Label, decimal Value, string DisplayValue, decimal ChangePercent, string ChangeLabel);
public sealed record DashboardPointDto(DateOnly Date, int Contributions);
public sealed record DashboardActivityDto(Guid Id, string ActionType, string Description, string EntityType, Guid? EntityId, string? ProjectName, string? UserName, DateTime CreatedAt);
public sealed record DashboardProjectDto(Guid Id, string Name, string? Description, string Language, int Progress, int MemberCount, int OpenTaskCount, DateTime UpdatedAt);
public sealed record DashboardDto(IReadOnlyList<DashboardMetricDto> Metrics, IReadOnlyList<DashboardPointDto> WeeklyProgress, IReadOnlyList<DashboardActivityDto> RecentActivity, IReadOnlyList<DashboardProjectDto> Projects);
public sealed record GetDashboardQuery : IRequest<DashboardDto>;
