using Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentDefinition;
using Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentDefinition;
using Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentDefinition;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Acknowledgments.Queries.GetAcknowledgmentDefinitionById;
using Eap.Application.Acknowledgments.Queries.ListAcknowledgmentDefinitions;
using Eap.Application.Common.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Acknowledgments;

/// <summary>
/// Acknowledgment definition endpoints. Controllers remain thin — every request
/// is delegated to a MediatR command/query so business logic stays in the
/// Application layer. Authoring requires <see cref="SystemRoles.AcknowledgmentManager"/>;
/// publishing is gated separately on the versions controller (SoD).
/// </summary>
[ApiController]
[Route("api/acknowledgments")]
[Authorize]
public sealed class AcknowledgmentDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcknowledgmentDefinitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AcknowledgmentDefinitionSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AcknowledgmentDefinitionSummaryDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] AcknowledgmentStatus? status = null,
        [FromQuery] string? ownerDepartment = null,
        [FromQuery] ActionType? actionType = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListAcknowledgmentDefinitionsQuery(page, pageSize, search, status, ownerDepartment, actionType),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{definitionId:guid}")]
    [ProducesResponseType(typeof(AcknowledgmentDefinitionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcknowledgmentDefinitionDetailDto>> GetById(
        Guid definitionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAcknowledgmentDefinitionByIdQuery(definitionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentDefinitionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AcknowledgmentDefinitionDetailDto>> Create(
        [FromBody] CreateAcknowledgmentDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateAcknowledgmentDefinitionCommand(
                request.Title,
                request.OwnerDepartment,
                request.DefaultActionType,
                request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { definitionId = result.Id }, result);
    }

    [HttpPut("{definitionId:guid}")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentDefinitionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcknowledgmentDefinitionDetailDto>> Update(
        Guid definitionId,
        [FromBody] UpdateAcknowledgmentDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateAcknowledgmentDefinitionCommand(
                definitionId,
                request.Title,
                request.OwnerDepartment,
                request.DefaultActionType,
                request.Description),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{definitionId:guid}/archive")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AcknowledgmentDefinitionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AcknowledgmentDefinitionDetailDto>> Archive(
        Guid definitionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ArchiveAcknowledgmentDefinitionCommand(definitionId),
            cancellationToken);
        return Ok(result);
    }
}
