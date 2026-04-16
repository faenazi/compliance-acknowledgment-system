namespace Eap.Application.Notifications.Commands.SendReminderNotifications;

/// <summary>
/// Configurable notification settings. Bound to <c>Notifications</c> in appsettings.
/// </summary>
public sealed class NotificationOptions
{
    public const string SectionName = "Notifications";

    /// <summary>Number of days before due date to send a reminder. Default: 7.</summary>
    public int ReminderDaysBeforeDue { get; set; } = 7;
}
