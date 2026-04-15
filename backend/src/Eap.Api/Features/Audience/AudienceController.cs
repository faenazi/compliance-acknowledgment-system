using Eap.Application.Audience.Commands.ConfigureAudienceExclusions;
using Eap.Application.Audience.Commands.ConfigureAudienceInclusion;
using Eap.Application.Audience.Commands.SetAllUsersAudience;
using Eap.Application.Audience.Models;
using Eap.Application.Audience.Queries.GetAudienceByVersion;
using Eap.Application.Audience.Queries.PreviewAudience;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Audience;

/// <summary>
/// Audience targeting endpoints (Sprint 4). Authoring is restricted to
/// <see cref="SystemRoles.AcknowledgmentManager"/> (segregation of duties —
/// publishers don't author audience rules).
/// </summary>
[ApiController]
[Route("api/acknowledgments/{definitionId:guid}/versions/{versionId:guid}/audience")]
[Authorize]
public sealed class AudienceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AudienceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AudienceDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudienceDefinitionDto>> Get(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAudienceByVersionQuery(definitionId, versionId),
            cancellationToken);

        return result is null ? NoContent() : Ok(result);
    }

    [HttpPut("inclusion")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AudienceDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudienceDefinitionDto>> ReplaceInclusion(
        Guid definitionId,
        Guid versionId,
        [FromBody] ConfigureAudienceInclusionRequest request,
        CancellationToken cancellationToken)
    {
        var rules = (request.Rules ?? Array.Empty<AudienceRuleRequest>())
            .Select(r => new AudienceRuleInputDto(r.RuleType, r.RuleValue))
            .ToList();

        var result = await _mediator.Send(
            new ConfigureAudienceInclusionCommand(definitionId, versionId, rules),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("exclusions")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AudienceDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudienceDefinitionDto>> ReplaceExclusions(
        Guid definitionId,
        Guid versionId,
        [FromBody] ConfigureAudienceExclusionsRequest request,
        CancellationToken cancellationToken)
    {
        var rules = (request.Rules ?? Array.Empty<AudienceRuleRequest>())
            .Select(r => new AudienceRuleInputDto(r.RuleType, r.RuleValue))
            .ToList();

        var result = await _mediator.Send(
            new ConfigureAudienceExclusionsCommand(definitionId, versionId, rules),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("all-users")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(AudienceDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudienceDefinitionDto>> SetAllUsers(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SetAllUsersAudienceCommand(definitionId, versionId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("preview")]
    [ProducesResponseType(typeof(AudiencePreviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudiencePreviewDto>> Preview(
        Guid definitionId,
        Guid versionId,
        [FromQuery] int sampleSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new PreviewAudienceQuery(definitionId, versionId, sampleSize),
            cancellationToken);
        return Ok(result);
    }
}
