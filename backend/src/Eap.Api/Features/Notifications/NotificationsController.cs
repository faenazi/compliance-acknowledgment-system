using Eap.Application.Common.Models;
using Eap.Application.Notifications.Commands.SendAssignmentNotifications;
using Eap.Application.Notifications.Commands.SendOverdueNotifications;
using Eap.Application.Notifications.Commands.SendReminderNotifications;
using Eap.Application.Notifications.Models;
using Eap.Application.Notifications.Queries.ListNotifications;
using Eap.Domain.Identity;
using Eap.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Notifications;

/// <summary>
/// Notification management endpoints (Sprint 8). Assignment, reminder, and
/// overdue notification triggers plus notification log listing.
/// </summary>
[ApiController]
[Route("api/admin/notifications")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.AcknowledgmentManager + ","
    + SystemRoles.PolicyManager)]
public sealed class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Send assignment notifications for pending requirements.</summary>
    [HttpPost("send-assignments")]
    [ProducesResponseType(typeof(NotificationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationResultDto>> SendAssignments(
        [FromQuery] Guid? acknowledgmentVersionId = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new SendAssignmentNotificationsCommand(acknowledgmentVersionId), ct);
        return Ok(result);
    }

    /// <summary>Send reminder notifications for requirements approaching due date.</summary>
    [HttpPost("send-reminders")]
    [ProducesResponseType(typeof(NotificationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationResultDto>> SendReminders(
        [FromQuery] int? reminderDaysBeforeDue = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new SendReminderNotificationsCommand(reminderDaysBeforeDue), ct);
        return Ok(result);
    }

    /// <summary>Send overdue notifications for requirements past due date.</summary>
    [HttpPost("send-overdue")]
    [ProducesResponseType(typeof(NotificationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationResultDto>> SendOverdue(
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SendOverdueNotificationsCommand(), ct);
        return Ok(result);
    }

    /// <summary>List notification log entries with optional filters.</summary>
    [HttpGet]
    [Authorize(Roles = SystemRoles.SystemAdministrator + ","
        + SystemRoles.ComplianceViewer + ","
        + SystemRoles.Auditor + ","
        + SystemRoles.AcknowledgmentManager + ","
        + SystemRoles.PolicyManager)]
    [ProducesResponseType(typeof(PagedResult<NotificationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<NotificationSummaryDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] NotificationType? type = null,
        [FromQuery] NotificationStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListNotificationsQuery(
            page, pageSize, type, status, search), ct);
        return Ok(result);
    }
}
