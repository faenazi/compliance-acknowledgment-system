using Eap.Application.Notifications.Models;
using MediatR;

namespace Eap.Application.Notifications.Commands.SendOverdueNotifications;

/// <summary>
/// Sends overdue notifications for requirements past their due date (FR-122).
/// </summary>
public sealed record SendOverdueNotificationsCommand : IRequest<NotificationResultDto>;
