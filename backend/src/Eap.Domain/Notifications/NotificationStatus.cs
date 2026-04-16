namespace Eap.Domain.Notifications;

/// <summary>
/// Delivery status of a notification (CDM §9.1, lifecycle §9).
/// </summary>
public enum NotificationStatus
{
    Queued = 0,
    Sent = 1,
    Failed = 2,
    Cancelled = 3,
}
