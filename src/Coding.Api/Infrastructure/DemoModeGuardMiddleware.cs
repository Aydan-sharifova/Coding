using System.IdentityModel.Tokens.Jwt;
using Coding.Application.Features.Demo;

namespace Coding.Api.Infrastructure;

public sealed class DemoModeGuardMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IDemoEnvironmentService demoEnvironment)
    {
        if (!demoEnvironment.IsEnabled)
        {
            await next(context);
            return;
        }

        var path = context.Request.Path;
        var blocksPublicAccountChanges =
            (HttpMethods.IsPost(context.Request.Method) &&
             path.Equals("/api/auth/register", StringComparison.OrdinalIgnoreCase)) ||
            path.StartsWithSegments("/api/auth/email-verification") ||
            path.StartsWithSegments("/api/auth/password");
        if (blocksPublicAccountChanges)
        {
            await RejectAsync(
                context,
                "Account and security changes are disabled in the public demo. Choose a demo role instead.");
            return;
        }

        var subject = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(subject, out var userId) ||
            !demoEnvironment.TryGetRole(userId, out _))
        {
            await next(context);
            return;
        }

        var blocksAdministration =
            path.StartsWithSegments("/api/admin") ||
            path.StartsWithSegments("/api/role") ||
            path.StartsWithSegments("/api/userrole");
        var blocksSecurityChanges =
            path.StartsWithSegments("/api/settings/password") ||
            path.StartsWithSegments("/api/settings/sessions");
        var blocksDestructiveChanges =
            HttpMethods.IsDelete(context.Request.Method) ||
            (path.StartsWithSegments("/api/projects") &&
             (path.Value?.Contains("/members", StringComparison.OrdinalIgnoreCase) == true ||
              path.Value?.Contains("/invitations", StringComparison.OrdinalIgnoreCase) == true));

        if (blocksAdministration || blocksSecurityChanges || blocksDestructiveChanges)
        {
            await RejectAsync(
                context,
                "This action is locked in the public demo to keep the showcase safe and reusable.");
            return;
        }

        await next(context);
    }

    private static async Task RejectAsync(HttpContext context, string detail)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Demo Environment restriction",
                detail: detail)
            .ExecuteAsync(context);
    }
}
