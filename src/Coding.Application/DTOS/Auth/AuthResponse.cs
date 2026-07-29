namespace Coding.DTOS.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    AuthenticatedUser User);

public sealed record AuthenticatedUser(
    Guid Id,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    bool IsEmailVerified,
    IReadOnlyCollection<string> Roles,
    bool IsDemo = false,
    string? DemoRole = null,
    Guid? DemoProjectId = null);
