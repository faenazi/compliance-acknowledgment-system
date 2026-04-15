using Eap.Application.Policies.Commands.ArchivePolicyVersion;
using Eap.Application.Policies.Commands.CreatePolicyVersion;
using Eap.Application.Policies.Commands.PublishPolicyVersion;
using Eap.Application.Policies.Commands.UpdatePolicyVersionDraft;
using Eap.Application.Policies.Models;
using Eap.Application.Policies.Queries.GetPolicyVersionById;
using Eap.Application.Policies.Queries.ListPolicyVersions;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Policies;

/// <summary>
/// Policy version endpoints. Authoring is gated to <see cref="SystemRoles.PolicyManager"/>;
/// publishing requires the dedicated <see cref="SystemRoles.Publisher"/> role
/// (segregation of duties — BR-041/BR-042-adjacent).
/// </summary>
[ApiController]
[Route("api/policies/{policyId:guid}/versions")]
[Authorize]
public sealed class PolicyVersionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PolicyVersionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PolicyVersionSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PolicyVersionSummaryDto>>> List(
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListPolicyVersionsQuery(policyId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{versionId:guid}")]
    [ProducesResponseType(typeof(PolicyVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolicyVersionDetailDto>> GetById(
        Guid policyId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPolicyVersionByIdQuery(policyId, versionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyVersionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolicyVersionDetailDto>> Create(
        Guid policyId,
        [FromBody] CreatePolicyVersionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreatePolicyVersionCommand(
                policyId,
                request.VersionLabel,
                request.EffectiveDate,
                request.Summary),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { policyId, versionId = result.Id },
            result);
    }

    [HttpPut("{versionId:guid}")]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PolicyVersionDetailDto>> UpdateDraft(
        Guid policyId,
        Guid versionId,
        [FromBody] UpdatePolicyVersionDraftRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdatePolicyVersionDraftCommand(
                policyId,
                versionId,
                request.VersionLabel,
                request.EffectiveDate,
                request.Summary),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{versionId:guid}/publish")]
    [Authorize(Roles = SystemRoles.Publisher + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PolicyVersionDetailDto>> Publish(
        Guid policyId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PublishPolicyVersionCommand(policyId, versionId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{versionId:guid}/archive")]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PolicyVersionDetailDto>> Archive(
        Guid policyId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ArchivePolicyVersionCommand(policyId, versionId),
            cancellationToken);

        return Ok(result);
    }
}
