using Eap.Application.Audit.Models;
using Eap.Application.Audit.Queries.ListAuditLogs;
using Eap.Application.Common.Models;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Audit;

/// <summary>
/// Audit log explorer endpoints (Sprint 8). Restricted to Auditor and
/// SystemAdministrator roles for governance and audit review.
/// </summary>
[ApiController]
[Route("api/admin/audit")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.Auditor + ","
    + SystemRoles.ComplianceViewer)]
public sealed class AuditController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Paginated, filtered list of audit log entries.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? actionType = null,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? actorUserId = null,
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListAuditLogsQuery(
            Page: page,
            PageSize: pageSize,
            ActionType: actionType,
            EntityType: entityType,
            ActorUserId: actorUserId,
            FromDate: fromDate,
            ToDate: toDate,
            Search: search), ct);
        return Ok(result);
    }
}
