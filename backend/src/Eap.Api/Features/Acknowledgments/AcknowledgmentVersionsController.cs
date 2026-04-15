using Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentVersion;
using Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentVersion;
using Eap.Application.Acknowledgments.Commands.PublishAcknowledgmentVersion;
using Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentVersionDraft;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Acknowledgments.Queries.GetAcknowledgmentVersionById;
using Eap.Application.Acknowledgments.Queries.ListAcknowledgmentVersions;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Acknowledgments;

/// <summary>
/// Acknowledgment version endpoints. Authoring is gated to
/// <see cref="SystemRoles.AcknowledgmentManager"/>; publishing requires the
/// dedicated <see cref="SystemRoles.Publisher"/> role (segregation of duties).
/// </summary>
[ApiController]
[Route("api/acknowledgments/{definitionId:guid}/versions")]
[Authorize]
public sealed class AcknowledgmentVersionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcknowledgmentVersionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AcknowledgmentVersionSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AcknowledgmentVersionSummaryDto>>> List(
        Guid definitionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ListAcknowledgmentVersionsQuery(definitionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{versionId:guid}")]
    [ProducesResponseType(typeof(AcknowledgmentVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcknowledgmentVersionDetailDto>> GetById(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAcknowledgmentVersionByIdQuery(definitionId, versionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentVersionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcknowledgmentVersionDetailDto>> Create(
        Guid definitionId,
        [FromBody] CreateAcknowledgmentVersionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateAcknowledgmentVersionCommand(
                definitionId,
                request.PolicyVersionId,
                request.ActionType,
                request.VersionLabel,
                request.Summary,
                request.CommitmentText,
                request.StartDate,
                request.DueDate),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { definitionId, versionId = result.Id },
            result);
    }

    [HttpPut("{versionId:guid}")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AcknowledgmentVersionDetailDto>> UpdateDraft(
        Guid definitionId,
        Guid versionId,
        [FromBody] UpdateAcknowledgmentVersionDraftRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateAcknowledgmentVersionDraftCommand(
                definitionId,
                versionId,
                request.PolicyVersionId,
                request.ActionType,
                request.VersionLabel,
                request.Summary,
                request.CommitmentText,
                request.StartDate,
                request.DueDate),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{versionId:guid}/publish")]
    [Authorize(Roles = SystemRoles.Publisher + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AcknowledgmentVersionDetailDto>> Publish(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new PublishAcknowledgmentVersionCommand(definitionId, versionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{versionId:guid}/archive")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentVersionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AcknowledgmentVersionDetailDto>> Archive(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ArchiveAcknowledgmentVersionCommand(definitionId, versionId),
            cancellationToken);
        return Ok(result);
    }
}
