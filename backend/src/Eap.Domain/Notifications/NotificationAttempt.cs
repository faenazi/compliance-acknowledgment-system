namespace Eap.Domain.Notifications;

/// <summary>
/// One attempt to deliver a notification (CDM §9.2).
/// Captures the result status and optional failure reason.
/// </summary>
public sealed class NotificationAttempt
{
    // EF Core
    private NotificationAttempt() { }

    internal NotificationAttempt(
        Guid notificationId,
        int attemptNumber,
        bool success,
        string? failureReason)
    {
        Id = Guid.NewGuid();
        NotificationId = notificationId;
        AttemptNumber = attemptNumber;
        AttemptedAtUtc = DateTimeOffset.UtcNow;
        Success = success;
        FailureReason = failureReason?.Trim();
    }

    public Guid Id { get; private set; }

    public Guid NotificationId { get; private set; }

    public int AttemptNumber { get; private set; }

    public DateTimeOffset AttemptedAtUtc { get; private set; }

    public bool Success { get; private set; }

    public string? FailureReason { get; private set; }
}
