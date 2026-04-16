using Eap.Application.Common.Models;
using Eap.Application.Compliance.Models;
using Eap.Application.Compliance.Queries.GetComplianceDashboard;
using Eap.Application.Compliance.Queries.ListNonCompliantUsers;
using Eap.Domain.Identity;
using Eap.Domain.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Compliance;

/// <summary>
/// Compliance dashboard and reporting endpoints (Sprint 8).
/// Restricted to ComplianceViewer, Auditor, and SystemAdministrator roles.
/// </summary>
[ApiController]
[Route("api/admin/compliance")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.ComplianceViewer + ","
    + SystemRoles.Auditor + ","
    + SystemRoles.PolicyManager + ","
    + SystemRoles.AcknowledgmentManager + ","
    + SystemRoles.Publisher)]
public sealed class ComplianceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComplianceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns the compliance dashboard summary with optional filters.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ComplianceDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ComplianceDashboardDto>> GetDashboard(
        [FromQuery] string? department = null,
        [FromQuery] Guid? acknowledgmentDefinitionId = null,
        [FromQuery] Guid? policyId = null,
        [FromQuery] int topNonCompliantLimit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetComplianceDashboardQuery(
            Department: department,
            AcknowledgmentDefinitionId: acknowledgmentDefinitionId,
            PolicyId: policyId,
            TopNonCompliantLimit: topNonCompliantLimit), cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a paginated list of non-compliant users.</summary>
    [HttpGet("non-compliant")]
    [ProducesResponseType(typeof(PagedResult<NonCompliantUserDetailDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<NonCompliantUserDetailDto>>> ListNonCompliantUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? department = null,
        [FromQuery] Guid? acknowledgmentDefinitionId = null,
        [FromQuery] Guid? policyId = null,
        [FromQuery] UserActionRequirementStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListNonCompliantUsersQuery(
            Page: page,
            PageSize: pageSize,
            Department: department,
            AcknowledgmentDefinitionId: acknowledgmentDefinitionId,
            PolicyId: policyId,
            Status: status,
            Search: search), cancellationToken);
        return Ok(result);
    }
}
