using Coding.Application.Abstractions;
using Coding.Application.Features.Analytics;
using Coding.Data;
using Coding.Enums;
using Coding.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Coding.Infrastructure.Analytics;

public sealed class GetAnalyticsDashboardHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetAnalyticsDashboardQuery, AnalyticsDashboardDto>
{
    public async Task<AnalyticsDashboardDto> Handle(GetAnalyticsDashboardQuery request, CancellationToken ct)
    {
        var to = (request.To ?? DateTime.UtcNow).ToUniversalTime();
        var from = (request.From ?? to.AddDays(-30)).ToUniversalTime();
        var memberProjects = db.ProjectMembers.AsNoTracking()
            .Where(x => x.UserId == currentUser.UserId)
            .Select(x => x.ProjectId);
        if (request.ProjectId.HasValue)
            memberProjects = memberProjects.Where(x => x == request.ProjectId.Value);

        var activity = db.ActivityLogs.AsNoTracking()
            .Where(x => x.CreatedAt >= from && x.CreatedAt <= to && x.ProjectId.HasValue && memberProjects.Contains(x.ProjectId.Value));
        var tasks = db.ProjectTasks.AsNoTracking().Where(x => memberProjects.Contains(x.ProjectId));
        var completedTasks = await tasks.CountAsync(x => x.Status == ProjectTaskStatus.Done, ct);
        var totalTasks = await tasks.CountAsync(ct);

        var activeUsers = await activity.Where(x => x.UserId.HasValue)
            .GroupBy(x => new { x.UserId, x.User!.FirstName, x.User.LastName, x.User.UserName, x.User.AvatarUrl })
            .Select(x => new ActiveUserDto(x.Key.UserId!.Value, x.Key.FirstName + " " + x.Key.LastName, x.Key.UserName, x.Key.AvatarUrl, x.Count()))
            .OrderByDescending(x => x.ActivityCount).Take(10).ToListAsync(ct);

        var projectsOverTime = await db.Projects.AsNoTracking()
            .Where(x => memberProjects.Contains(x.ID) && x.CreatedAt >= from && x.CreatedAt <= to)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(x => new TimeSeriesPointDto(x.Key, x.Count()))
            .OrderBy(x => x.Period).ToListAsync(ct);

        var languages = await db.Projects.AsNoTracking()
            .Where(x => memberProjects.Contains(x.ID))
            .GroupBy(x => x.DefaultLanguage)
            .Select(x => new LanguageUsageDto(x.Key == "" ? "Other" : x.Key, x.Count()))
            .OrderByDescending(x => x.ProjectCount).Take(8).ToListAsync(ct);

        var weekly = await activity
            .GroupBy(x => x.CreatedAt.Date)
            .Select(x => new TimeSeriesPointDto(x.Key, x.Count()))
            .OrderBy(x => x.Period).ToListAsync(ct);

        var monthly = await activity
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(x => new TimeSeriesPointDto(new DateTime(x.Key.Year, x.Key.Month, 1, 0, 0, 0, DateTimeKind.Utc), x.Count()))
            .OrderBy(x => x.Period).ToListAsync(ct);

        var fileChanges = await db.FileVersions.AsNoTracking()
            .CountAsync(x => memberProjects.Contains(x.Node.ProjectId) && x.CreatAt >= from && x.CreatAt <= to, ct);
        var projectsCreated = await db.Projects.AsNoTracking()
            .CountAsync(x => memberProjects.Contains(x.ID) && x.CreatedAt >= from && x.CreatedAt <= to, ct);
        var sessionMinutes = await db.CodingSessions.AsNoTracking()
            .Where(x => memberProjects.Contains(x.ProjectId) && x.StartAt <= to && (x.EndAt == null || x.EndAt >= from))
            .SumAsync(x => (double?)Math.Min(
                ((x.EndAt ?? x.LastActivityAt) - x.StartAt).TotalMinutes,
                30), ct) ?? 0;

        return new AnalyticsDashboardDto(from, to,
            new AnalyticsSummaryDto(
                activeUsers.Count,
                projectsCreated,
                totalTasks == 0 ? 0 : Math.Round(100m * completedTasks / totalTasks, 1),
                fileChanges,
                Math.Round((decimal)sessionMinutes / 60m, 1)),
            activeUsers, projectsOverTime, languages, weekly, monthly);
    }
}

public sealed class StartCodingSessionHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<StartCodingSessionCommand, Guid>
{
    public async Task<Guid> Handle(StartCodingSessionCommand request, CancellationToken ct)
    {
        var allowed = await db.ProjectMembers.AsNoTracking()
            .AnyAsync(x => x.ProjectId == request.ProjectId && x.UserId == currentUser.UserId, ct);
        var validFile = await db.WorkspaceNodes.AsNoTracking()
            .AnyAsync(x => x.ID == request.FileId && x.ProjectId == request.ProjectId && x.NodeType == WorkspaceNodeType.File, ct);
        if (!allowed || !validFile) throw new UnauthorizedAccessException("Project membership is required.");

        var now = DateTime.UtcNow;
        var existing = await db.CodingSessions
            .Where(x => x.UserId == currentUser.UserId && x.FileId == request.FileId && x.EndAt == null)
            .OrderByDescending(x => x.StartAt).FirstOrDefaultAsync(ct);
        if (existing is not null && existing.LastActivityAt >= now.AddMinutes(-2))
        {
            existing.LastActivityAt = now;
            await db.SaveChangesAsync(ct);
            return existing.Id;
        }
        if (existing is not null) existing.EndAt = existing.LastActivityAt;

        var session = new CodingSession
        {
            Id = Guid.NewGuid(), UserId = currentUser.UserId, ProjectId = request.ProjectId,
            FileId = request.FileId, StartAt = now, LastActivityAt = now
        };
        db.CodingSessions.Add(session);
        await db.SaveChangesAsync(ct);
        return session.Id;
    }
}

public sealed class HeartbeatCodingSessionHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<HeartbeatCodingSessionCommand>
{
    public async Task Handle(HeartbeatCodingSessionCommand request, CancellationToken ct)
    {
        var session = await db.CodingSessions.SingleOrDefaultAsync(x => x.Id == request.SessionId && x.UserId == currentUser.UserId && x.EndAt == null, ct)
            ?? throw new KeyNotFoundException("Coding session was not found.");
        var now = DateTime.UtcNow;
        if (session.LastActivityAt < now.AddMinutes(-5))
            session.EndAt = session.LastActivityAt;
        else
            session.LastActivityAt = now;
        await db.SaveChangesAsync(ct);
    }
}

public sealed class EndCodingSessionHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<EndCodingSessionCommand>
{
    public async Task Handle(EndCodingSessionCommand request, CancellationToken ct)
    {
        var session = await db.CodingSessions.SingleOrDefaultAsync(x => x.Id == request.SessionId && x.UserId == currentUser.UserId && x.EndAt == null, ct);
        if (session is null) return;
        session.EndAt = DateTime.UtcNow < session.LastActivityAt.AddMinutes(5) ? DateTime.UtcNow : session.LastActivityAt;
        await db.SaveChangesAsync(ct);
    }
}
