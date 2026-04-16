using Eap.Application.Notifications.Models;
using MediatR;

namespace Eap.Application.Notifications.Commands.SendReminderNotifications;

/// <summary>
/// Sends reminder notifications for requirements approaching their due date (FR-121).
/// The reminder window is configurable via <c>Notifications:ReminderDaysBeforeDue</c>.
/// </summary>
public sealed record SendReminderNotificationsCommand(
    int? ReminderDaysBeforeDue = null) : IRequest<NotificationResultDto>;
