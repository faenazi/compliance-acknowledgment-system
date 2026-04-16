namespace Eap.Domain.Notifications;

/// <summary>
/// Types of platform notifications (BR-111, CDM §9.1).
/// </summary>
public enum NotificationType
{
    /// <summary>Sent when a new action is assigned to a user.</summary>
    Assignment = 0,

    /// <summary>Sent as a reminder before the due date.</summary>
    Reminder = 1,

    /// <summary>Sent after the due date has passed without completion.</summary>
    Overdue = 2,
}
