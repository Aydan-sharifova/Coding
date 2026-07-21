namespace Coding.Services.Interfaces;

public interface IEmailSender
{
    Task SendEmailVerificationAsync(string email, string token, CancellationToken cancellationToken);
    Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken);
}
