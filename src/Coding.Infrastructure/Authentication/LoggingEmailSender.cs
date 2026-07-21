using Coding.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Coding.Infrastructure.Authentication;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        this.logger = logger;
    }

    public Task SendEmailVerificationAsync(
        string email,
        string token,
        CancellationToken cancellationToken)
    {
        logger.LogWarning(
            "Email delivery provider is not configured. Verification email for {Email} was not delivered.",
            email);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(
        string email,
        string token,
        CancellationToken cancellationToken)
    {
        logger.LogWarning(
            "Email delivery provider is not configured. Password reset email for {Email} was not delivered.",
            email);
        return Task.CompletedTask;
    }
}
