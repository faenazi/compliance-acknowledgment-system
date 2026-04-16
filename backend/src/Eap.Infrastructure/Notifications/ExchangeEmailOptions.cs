using System.ComponentModel.DataAnnotations;

namespace Eap.Infrastructure.Notifications;

/// <summary>
/// Configurable Exchange / SMTP transport settings. All values are
/// externalised so no sender/transport values are hardcoded.
/// Bound to <c>Exchange</c> section in appsettings.
/// </summary>
public sealed class ExchangeEmailOptions
{
    public const string SectionName = "Exchange";

    /// <summary>Master switch — when false, emails are logged but not sent.</summary>
    public bool Enabled { get; set; }

    /// <summary>SMTP host (Exchange relay or SMTP server).</summary>
    [Required]
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>SMTP port. Default: 587.</summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>Use SSL/TLS for SMTP connection.</summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>Sender email address.</summary>
    [Required]
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>Display name for the sender.</summary>
    public string SenderDisplayName { get; set; } = "منصة الإقرارات";

    /// <summary>SMTP username (leave empty for default credentials).</summary>
    public string? Username { get; set; }

    /// <summary>SMTP password (should be provided via secrets/env vars).</summary>
    public string? Password { get; set; }

    /// <summary>Use Windows default credentials (service account). Default: false.</summary>
    public bool UseDefaultCredentials { get; set; }

    /// <summary>SMTP send timeout in milliseconds. Default: 30000.</summary>
    public int TimeoutMs { get; set; } = 30_000;
}
