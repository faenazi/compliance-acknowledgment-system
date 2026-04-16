using Eap.Application.Admin.Models;
using Eap.Application.Admin.Queries.GetAdminRequirementDetail;
using Eap.Application.Admin.Queries.GetAdminSubmissionDetail;
using Eap.Application.Admin.Queries.ListUserRequirements;
using Eap.Application.Common.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Identity;
using Eap.Domain.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Admin;

/// <summary>
/// Admin monitoring endpoints for user action requirements and submission review (Sprint 7).
/// Restricted to admin-capable roles.
/// </summary>
[ApiController]
[Route("api/admin/monitoring")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.PolicyManager + ","
    + SystemRoles.AcknowledgmentManager + ","
    + SystemRoles.Publisher + ","
    + SystemRoles.ComplianceViewer + ","
    + SystemRoles.Auditor)]
public sealed class AdminMonitoringController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminMonitoringController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Paginated list of user action requirements with admin filters.</summary>
    [HttpGet("requirements")]
    [ProducesResponseType(typeof(PagedResult<AdminRequirementSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminRequirementSummaryDto>>> ListRequirements(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] UserActionRequirementStatus? status = null,
        [FromQuery] Guid? acknowledgmentDefinitionId = null,
        [FromQuery] Guid? policyId = null,
        [FromQuery] string? department = null,
        [FromQuery] RecurrenceModel? recurrenceModel = null,
        [FromQuery] DateOnly? dueDateFrom = null,
        [FromQuery] DateOnly? dueDateTo = null,
        [FromQuery] string? search = null,
        [FromQuery] bool currentOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListUserRequirementsQuery(
            Page: page,
            PageSize: pageSize,
            Status: status,
            AcknowledgmentDefinitionId: acknowledgmentDefinitionId,
            PolicyId: policyId,
            Department: department,
            RecurrenceModel: recurrenceModel,
            DueDateFrom: dueDateFrom,
            DueDateTo: dueDateTo,
            Search: search,
            CurrentOnly: currentOnly), cancellationToken);
        return Ok(result);
    }

    /// <summary>Full detail for a single user action requirement.</summary>
    [HttpGet("requirements/{requirementId:guid}")]
    [ProducesResponseType(typeof(AdminRequirementDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminRequirementDetailDto>> GetRequirementDetail(
        Guid requirementId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAdminRequirementDetailQuery(requirementId), cancellationToken);
        return Ok(result);
    }

    /// <summary>Full detail for a submission (admin review, not scoped to current user).</summary>
    [HttpGet("submissions/{submissionId:guid}")]
    [ProducesResponseType(typeof(AdminSubmissionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSubmissionDetailDto>> GetSubmissionDetail(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAdminSubmissionDetailQuery(submissionId), cancellationToken);
        return Ok(result);
    }
}
