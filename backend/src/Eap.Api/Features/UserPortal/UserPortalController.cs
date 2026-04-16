using Eap.Application.Common.Models;
using Eap.Application.UserPortal.Commands.SubmitAcknowledgment;
using Eap.Application.UserPortal.Commands.SubmitDisclosure;
using Eap.Application.UserPortal.Models;
using Eap.Application.UserPortal.Queries.GetMyActionDetail;
using Eap.Application.UserPortal.Queries.GetMyActions;
using Eap.Application.UserPortal.Queries.GetMyDashboard;
using Eap.Application.UserPortal.Queries.GetMyHistory;
using Eap.Application.UserPortal.Queries.GetMySubmissionDetail;
using Eap.Domain.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.UserPortal;

/// <summary>
/// Employee-facing user portal endpoints (Sprint 6). Any authenticated user
/// may access these — no admin role restriction. All queries are scoped to
/// the current user's identity automatically.
/// </summary>
[ApiController]
[Authorize]
[Route("api/me")]
public sealed class UserPortalController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserPortalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Dashboard summary: counts + recent pending + recently completed.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(MyDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MyDashboardDto>> GetDashboard(
        [FromQuery] int pendingLimit = 5,
        [FromQuery] int recentLimit = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetMyDashboardQuery(pendingLimit, recentLimit),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Paginated list of the current user's action requirements.</summary>
    [HttpGet("actions")]
    [ProducesResponseType(typeof(PagedResult<MyActionSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MyActionSummaryDto>>> ListActions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] UserActionRequirementStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetMyActionsQuery(page, pageSize, status, search),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Full detail for a single action requirement.</summary>
    [HttpGet("actions/{requirementId:guid}")]
    [ProducesResponseType(typeof(MyActionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyActionDetailDto>> GetActionDetail(
        Guid requirementId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMyActionDetailQuery(requirementId),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Submit a simple or commitment acknowledgment.</summary>
    [HttpPost("actions/{requirementId:guid}/acknowledge")]
    [ProducesResponseType(typeof(SubmissionResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmissionResultDto>> SubmitAcknowledgment(
        Guid requirementId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SubmitAcknowledgmentCommand(requirementId),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Submit a form-based disclosure.</summary>
    [HttpPost("actions/{requirementId:guid}/disclose")]
    [ProducesResponseType(typeof(SubmissionResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmissionResultDto>> SubmitDisclosure(
        Guid requirementId,
        [FromBody] SubmitDisclosureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SubmitDisclosureCommand(requirementId, request.SubmissionJson),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Paginated submission history.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PagedResult<MyHistoryItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MyHistoryItemDto>>> ListHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetMyHistoryQuery(page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Full detail for a past submission.</summary>
    [HttpGet("history/{submissionId:guid}")]
    [ProducesResponseType(typeof(MySubmissionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MySubmissionDetailDto>> GetSubmissionDetail(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMySubmissionDetailQuery(submissionId),
            cancellationToken);
        return Ok(result);
    }
}

/// <summary>Request body for form-based disclosure submission.</summary>
public sealed class SubmitDisclosureRequest
{
    public string SubmissionJson { get; set; } = default!;
}
