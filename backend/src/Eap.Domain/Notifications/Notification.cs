using Eap.Domain.Common;

namespace Eap.Domain.Notifications;

/// <summary>
/// A logical notification created by the system for a specific user and action
/// (CDM §9.1, BR-110 to BR-114). Tracks delivery status and owns one or more
/// <see cref="NotificationAttempt"/> records.
/// </summary>
public sealed class Notification : Entity
{
    private readonly List<NotificationAttempt> _attempts = new();

    // EF Core
    private Notification() { }

    public Notification(
        Guid userId,
        string recipientEmail,
        NotificationType notificationType,
        string relatedEntityType,
        Guid relatedEntityId,
        string subject,
        string bodyHtml)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new ArgumentException("Recipient email is required.", nameof(recipientEmail));
        }

        UserId = userId;
        RecipientEmail = recipientEmail.Trim();
        NotificationType = notificationType;
        RelatedEntityType = relatedEntityType?.Trim() ?? string.Empty;
        RelatedEntityId = relatedEntityId;
        Subject = subject?.Trim() ?? string.Empty;
        BodyHtml = bodyHtml ?? string.Empty;
        Status = NotificationStatus.Queued;
        ScheduledAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid UserId { get; private set; }

    public string RecipientEmail { get; private set; } = default!;

    public NotificationType NotificationType { get; private set; }

    /// <summary>Type of the related entity — e.g. UserActionRequirement.</summary>
    public string RelatedEntityType { get; private set; } = default!;

    /// <summary>Primary key of the related entity.</summary>
    public Guid RelatedEntityId { get; private set; }

    /// <summary>Email subject line.</summary>
    public string Subject { get; private set; } = default!;

    /// <summary>Email body content (HTML).</summary>
    public string BodyHtml { get; private set; } = default!;

    public NotificationStatus Status { get; private set; }

    public DateTimeOffset ScheduledAtUtc { get; private set; }

    public DateTimeOffset? SentAtUtc { get; private set; }

    public IReadOnlyList<NotificationAttempt> Attempts => _attempts.AsReadOnly();

    /// <summary>Mark the notification as successfully sent.</summary>
    public void MarkSent(DateTimeOffset whenUtc)
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = whenUtc;
        UpdatedAtUtc = whenUtc;
    }

    /// <summary>Mark the notification as failed after exhausting retries.</summary>
    public void MarkFailed(DateTimeOffset whenUtc)
    {
        Status = NotificationStatus.Failed;
        UpdatedAtUtc = whenUtc;
    }

    /// <summary>Cancel a queued notification that is no longer needed.</summary>
    public void Cancel(DateTimeOffset whenUtc)
    {
        if (Status == NotificationStatus.Sent) return;
        Status = NotificationStatus.Cancelled;
        UpdatedAtUtc = whenUtc;
    }

    /// <summary>Record an attempt to deliver this notification.</summary>
    public NotificationAttempt RecordAttempt(bool success, string? failureReason)
    {
        var attempt = new NotificationAttempt(
            Id,
            _attempts.Count + 1,
            success,
            failureReason);
        _attempts.Add(attempt);
        return attempt;
    }
}
