namespace Eap.Application.Notifications.Abstractions;

/// <summary>
/// Abstraction for sending email messages via Exchange / SMTP (BR-110, FR-123).
/// Transport configuration is environment-driven so the same code works
/// against Exchange, SMTP relay, or a test stub.
/// </summary>
public interface IEmailSender
{
    /// <summary>Send an email message. Returns true on success, false on failure.</summary>
    Task<(bool Success, string? FailureReason)> SendAsync(
        string toEmail,
        string subject,
        string bodyHtml,
        CancellationToken ct);
}
