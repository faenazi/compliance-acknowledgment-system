using Eap.Application.Requirements.Commands.GenerateRequirementsForVersion;
using Eap.Application.Requirements.Models;
using Eap.Application.Requirements.Queries.ListRequirementsForVersion;
using Eap.Domain.Identity;
using Eap.Domain.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Requirements;

/// <summary>
/// Requirement generation + listing endpoints (Sprint 4). Triggering generation
/// is restricted to <see cref="SystemRoles.AcknowledgmentManager"/>; viewing is
/// open to any authenticated admin role.
/// </summary>
[ApiController]
[Route("api/acknowledgments/{definitionId:guid}/versions/{versionId:guid}/requirements")]
[Authorize]
public sealed class RequirementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequirementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserActionRequirementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserActionRequirementDto>>> List(
        Guid definitionId,
        Guid versionId,
        [FromQuery] UserActionRequirementStatus? status,
        [FromQuery] bool currentOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListRequirementsForVersionQuery(definitionId, versionId, status, currentOnly),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("generate")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(RequirementGenerationSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequirementGenerationSummaryDto>> Generate(
        Guid definitionId,
        Guid versionId,
        [FromBody] GenerateRequirementsRequest? request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GenerateRequirementsForVersionCommand(definitionId, versionId, request?.CycleReference),
            cancellationToken);
        return Ok(result);
    }
}

public sealed class GenerateRequirementsRequest
{
    /// <summary>Optional override for event-driven or on-change cadences.</summary>
    public string? CycleReference { get; set; }
}
