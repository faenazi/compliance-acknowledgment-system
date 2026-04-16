using Eap.Application.Notifications.Models;
using MediatR;

namespace Eap.Application.Notifications.Commands.SendAssignmentNotifications;

/// <summary>
/// Sends assignment notifications for newly generated requirements (FR-120).
/// Optionally scoped to a specific acknowledgment version.
/// </summary>
public sealed record SendAssignmentNotificationsCommand(
    Guid? AcknowledgmentVersionId = null) : IRequest<NotificationResultDto>;
