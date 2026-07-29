using Coding.Exceptions;

namespace Coding.Application.Features.Demo;

public enum DemoRole
{
    Owner,
    Admin,
    Member
}

public interface IDemoEnvironmentService
{
    bool IsEnabled { get; }
    Guid SampleProjectId { get; }
    int AccessTokenMinutes { get; }
    int RefreshTokenHours { get; }

    void EnsureAvailable();
    Guid GetUserId(DemoRole role);
    bool TryGetRole(Guid userId, out DemoRole role);
    void EnsureFileAllowed(Guid userId, string fileName, long contentLength);
    Task ResetAsync(CancellationToken cancellationToken = default);
}

public sealed class DisabledDemoEnvironmentService : IDemoEnvironmentService
{
    public bool IsEnabled => false;
    public Guid SampleProjectId => Guid.Empty;
    public int AccessTokenMinutes => 0;
    public int RefreshTokenHours => 0;

    public void EnsureAvailable() =>
        throw new NotFoundException("The demo login endpoint is not available.");

    public Guid GetUserId(DemoRole role)
    {
        EnsureAvailable();
        return Guid.Empty;
    }

    public bool TryGetRole(Guid userId, out DemoRole role)
    {
        role = default;
        return false;
    }

    public void EnsureFileAllowed(Guid userId, string fileName, long contentLength)
    {
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        EnsureAvailable();
        return Task.CompletedTask;
    }
}
