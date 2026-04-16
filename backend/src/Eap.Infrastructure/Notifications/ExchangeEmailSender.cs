using System.Net;
using System.Net.Mail;
using Eap.Application.Notifications.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Notifications;

/// <summary>
/// Sends email via Exchange/SMTP (FR-123, BR-110). All transport configuration
/// is externalised through <see cref="ExchangeEmailOptions"/> so no values
/// are hardcoded (deployment constraint).
/// </summary>
internal sealed class ExchangeEmailSender : IEmailSender
{
    private readonly ExchangeEmailOptions _options;
    private readonly ILogger<ExchangeEmailSender> _logger;

    public ExchangeEmailSender(
        IOptions<ExchangeEmailOptions> options,
        ILogger<ExchangeEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<(bool Success, string? FailureReason)> SendAsync(
        string toEmail, string subject, string bodyHtml, CancellationToken ct)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation(
                "Email sending disabled. Would have sent to {To}: {Subject}", toEmail, subject);
            return (true, null);
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_options.SenderEmail, _options.SenderDisplayName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true,
            };
            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = _options.TimeoutMs,
            };

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                client.Credentials = new NetworkCredential(
                    _options.Username, _options.Password);
            }
            else
            {
                client.UseDefaultCredentials = _options.UseDefaultCredentials;
            }

            await client.SendMailAsync(message, ct);

            _logger.LogInformation(
                "{AuditEvent} EmailSent {To} {Subject}",
                "EmailSent", toEmail, subject);

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "{AuditEvent} EmailSendFailed {To} {Subject} {Error}",
                "EmailSendFailed", toEmail, subject, ex.Message);
            return (false, ex.Message);
        }
    }
}
