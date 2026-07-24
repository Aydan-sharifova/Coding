using System.Net;
using System.Net.Mail;
using System.Text;
using Coding.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Coding.Infrastructure.Authentication;

public sealed class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public Task SendEmailVerificationAsync(string email, string token, CancellationToken ct) =>
        SendAsync(email, "Verify your Coding email", "Verify email", "Confirm your email address to activate all workspace features.", $"/verify-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}", ct);

    public Task SendPasswordResetAsync(string email, string token, CancellationToken ct) =>
        SendAsync(email, "Reset your Coding password", "Reset password", "Use this secure link to choose a new password. If you did not request this, ignore the message.", $"/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}", ct);

    private async Task SendAsync(string recipient, string subject, string heading, string description, string path, CancellationToken ct)
    {
        var settings = options.Value; var link = $"{settings.ClientBaseUrl.TrimEnd('/')}{path}";
        using var message = new MailMessage { From = new MailAddress(settings.FromEmail, settings.FromName, Encoding.UTF8), Subject = subject, IsBodyHtml = true, BodyEncoding = Encoding.UTF8, SubjectEncoding = Encoding.UTF8, Body = Template(heading, description, link) };
        message.To.Add(new MailAddress(recipient));
        using var client = new SmtpClient(settings.Host, settings.Port) { EnableSsl = settings.EnableSsl, DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false, Credentials = string.IsNullOrWhiteSpace(settings.Username) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(settings.Username, settings.Password), Timeout = 15000 };
        logger.LogInformation("Sending {EmailType} email to {Recipient} through SMTP host {Host}.", heading, recipient, settings.Host);
        await client.SendMailAsync(message, ct);
    }

    private static string Template(string heading, string description, string link) => $"""
        <!doctype html><html><body style="margin:0;background:#f4f6fb;font-family:Arial,sans-serif;color:#152039">
        <table role="presentation" width="100%" cellspacing="0" cellpadding="0"><tr><td align="center" style="padding:32px 16px">
        <table role="presentation" width="560" cellspacing="0" cellpadding="0" style="max-width:100%;background:white;border:1px solid #e2e6ef;border-radius:14px">
        <tr><td style="padding:32px"><div style="font-size:20px;font-weight:800;color:#6256e8">Coding</div>
        <h1 style="font-size:24px;margin:28px 0 12px">{WebUtility.HtmlEncode(heading)}</h1><p style="color:#667085;line-height:1.6">{WebUtility.HtmlEncode(description)}</p>
        <a href="{WebUtility.HtmlEncode(link)}" style="display:inline-block;margin-top:16px;padding:13px 20px;border-radius:8px;background:#6256e8;color:white;text-decoration:none;font-weight:700">{WebUtility.HtmlEncode(heading)}</a>
        <p style="margin-top:28px;color:#98a2b3;font-size:12px;word-break:break-all">If the button does not work, open: {WebUtility.HtmlEncode(link)}</p></td></tr></table>
        </td></tr></table></body></html>
        """;
}
