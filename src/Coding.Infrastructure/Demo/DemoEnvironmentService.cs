using Coding.Application.Features.Demo;
using Coding.Data;
using Coding.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Coding.Infrastructure.Demo;

public sealed class DemoEnvironmentService(
    AppDbContext db,
    DemoDataSeeder seeder,
    IHostEnvironment environment,
    IOptions<DemoModeOptions> options,
    ILogger<DemoEnvironmentService> logger) : IDemoEnvironmentService
{
    private const long AdvisoryLockId = 2_026_072_900_17;
    private static readonly SemaphoreSlim ProcessResetGate = new(1, 1);
    private readonly DemoModeOptions settings = options.Value;

    public bool IsEnabled =>
        settings.Enabled &&
        environment.IsEnvironment("Demo");

    public Guid SampleProjectId => DemoDataIds.ProjectId;
    public int AccessTokenMinutes => Math.Clamp(settings.AccessTokenMinutes, 5, 60);
    public int RefreshTokenHours => Math.Clamp(settings.RefreshTokenHours, 1, 12);

    public void EnsureAvailable()
    {
        if (!settings.Enabled || !environment.IsEnvironment("Demo"))
            throw new NotFoundException("The demo login endpoint is not available.");

        var connectionString = db.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("The demo database connection is unavailable.");

        var databaseName = new NpgsqlConnectionStringBuilder(connectionString).Database;
        if (string.IsNullOrWhiteSpace(settings.DatabaseNameMarker) ||
            string.IsNullOrWhiteSpace(databaseName) ||
            !databaseName.Contains(settings.DatabaseNameMarker, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "DemoMode refused to start because its database does not contain the configured demo marker.");
        }
    }

    public Guid GetUserId(DemoRole role)
    {
        EnsureAvailable();
        return role switch
        {
            DemoRole.Owner => DemoDataIds.OwnerUserId,
            DemoRole.Admin => DemoDataIds.AdminUserId,
            DemoRole.Member => DemoDataIds.MemberUserId,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Unsupported demo role.")
        };
    }

    public bool TryGetRole(Guid userId, out DemoRole role)
    {
        if (!IsEnabled)
        {
            role = default;
            return false;
        }

        if (userId == DemoDataIds.OwnerUserId)
        {
            role = DemoRole.Owner;
            return true;
        }
        if (userId == DemoDataIds.AdminUserId)
        {
            role = DemoRole.Admin;
            return true;
        }
        if (userId == DemoDataIds.MemberUserId)
        {
            role = DemoRole.Member;
            return true;
        }

        role = default;
        return false;
    }

    public void EnsureFileAllowed(Guid userId, string fileName, long contentLength)
    {
        if (!TryGetRole(userId, out _))
            return;

        var maximumBytes = Math.Clamp(settings.MaxUploadBytes, 64 * 1024, 5 * 1024 * 1024);
        if (contentLength <= 0 || contentLength > maximumBytes)
            throw new FluentValidation.ValidationException(
                $"Demo uploads must be between 1 byte and {maximumBytes / 1024:N0} KB.");

        var extension = Path.GetExtension(Path.GetFileName(fileName));
        if (settings.BlockedFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            throw new FluentValidation.ValidationException(
                $"{extension} files are disabled in the public demo.");
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        EnsureAvailable();
        if (!await ProcessResetGate.WaitAsync(0, cancellationToken))
            throw new ConflictException("A demo reset is already running.");

        var resetId = Guid.NewGuid();
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["DemoResetId"] = resetId,
            ["DemoProjectId"] = DemoDataIds.ProjectId,
            ["Environment"] = environment.EnvironmentName
        });

        try
        {
            logger.LogInformation("Demo reset started");
            var executionStrategy = db.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await db.Database.BeginTransactionAsync(cancellationToken);

                var acquired = await db.Database
                    .SqlQueryRaw<bool>(
                        $"SELECT pg_try_advisory_xact_lock({AdvisoryLockId}) AS \"Value\"")
                    .SingleAsync(cancellationToken);
                if (!acquired)
                    throw new ConflictException("A demo reset is already running on another instance.");

                var demoUserIds = DemoDataIds.UserIds;
                var projectIds = await db.Projects
                    .IgnoreQueryFilters()
                    .Where(project =>
                        demoUserIds.Contains(project.OwnerId) ||
                        project.ID == DemoDataIds.ProjectId)
                    .Select(project => project.ID)
                    .ToListAsync(cancellationToken);

                var conversationIds = await db.ConversationParticipants
                    .IgnoreQueryFilters()
                    .Where(participant => demoUserIds.Contains(participant.UserId))
                    .Select(participant => participant.ConversationId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                await db.ActivityLogs
                    .Where(item =>
                        (item.ProjectId.HasValue && projectIds.Contains(item.ProjectId.Value)) ||
                        (item.UserId.HasValue && demoUserIds.Contains(item.UserId.Value)))
                    .ExecuteDeleteAsync(cancellationToken);

                if (conversationIds.Count > 0)
                {
                    await db.Conversations
                        .IgnoreQueryFilters()
                        .Where(item => conversationIds.Contains(item.ID))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                if (projectIds.Count > 0)
                {
                    await db.FileContents
                        .IgnoreQueryFilters()
                        .Where(content => projectIds.Contains(content.Node.ProjectId))
                        .ExecuteDeleteAsync(cancellationToken);
                    await db.FileVersions
                        .IgnoreQueryFilters()
                        .Where(version => projectIds.Contains(version.Node.ProjectId))
                        .ExecuteDeleteAsync(cancellationToken);
                    await db.CodingSessions
                        .Where(session => projectIds.Contains(session.ProjectId))
                        .ExecuteDeleteAsync(cancellationToken);

                    while (await db.WorkspaceNodes
                               .IgnoreQueryFilters()
                               .AnyAsync(node => projectIds.Contains(node.ProjectId), cancellationToken))
                    {
                        var deleted = await db.WorkspaceNodes
                            .IgnoreQueryFilters()
                            .Where(node =>
                                projectIds.Contains(node.ProjectId) &&
                                !db.WorkspaceNodes
                                    .IgnoreQueryFilters()
                                    .Any(child => child.ParentId == node.ID))
                            .ExecuteDeleteAsync(cancellationToken);
                        if (deleted == 0)
                            throw new InvalidOperationException(
                                "Demo reset found a cyclic workspace hierarchy.");
                    }

                    await db.Projects
                        .IgnoreQueryFilters()
                        .Where(project => projectIds.Contains(project.ID))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                await db.Users
                    .IgnoreQueryFilters()
                    .Where(user => demoUserIds.Contains(user.ID))
                    .ExecuteDeleteAsync(cancellationToken);

                db.ChangeTracker.Clear();
                await seeder.SeedAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });

            logger.LogInformation("Demo reset completed successfully");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Demo reset failed");
            throw;
        }
        finally
        {
            ProcessResetGate.Release();
        }
    }
}

public sealed class DemoResetBackgroundService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    IOptions<DemoModeOptions> options,
    ILogger<DemoResetBackgroundService> logger) : BackgroundService
{
    private readonly DemoModeOptions settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!settings.Enabled || !environment.IsEnvironment("Demo") ||
            settings.ResetIntervalMinutes <= 0)
            return;

        var interval = TimeSpan.FromMinutes(
            Math.Clamp(settings.ResetIntervalMinutes, 15, 24 * 60));
        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var demo = scope.ServiceProvider.GetRequiredService<IDemoEnvironmentService>();
                await demo.ResetAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (ConflictException exception)
            {
                logger.LogInformation(exception, "Scheduled demo reset skipped");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Scheduled demo reset failed");
            }
        }
    }
}
