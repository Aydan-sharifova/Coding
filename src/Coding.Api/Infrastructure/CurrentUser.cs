using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Coding.Application.Abstractions;
using Coding.Exceptions;

namespace Coding.Api.Infrastructure;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => accessor.HttpContext?.User ?? throw new UnauthorizedException("Authentication is required.");
    public Guid UserId => Guid.TryParse(Principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : throw new UnauthorizedException("The authenticated user identifier is invalid.");
    public string Email => Principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? throw new UnauthorizedException("The authenticated email is unavailable.");
}
