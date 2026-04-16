using Eap.Application.Forms.Commands.ConfigureFormDefinition;
using Eap.Application.Forms.Models;
using Eap.Application.Forms.Queries.GetFormDefinitionByVersion;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Forms;

/// <summary>
/// Form definition endpoints (Sprint 5). Authoring is restricted to
/// <see cref="SystemRoles.AcknowledgmentManager"/> (same SoD as audience).
/// </summary>
[ApiController]
[Route("api/acknowledgments/{definitionId:guid}/versions/{versionId:guid}/form")]
[Authorize]
public sealed class FormDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FormDefinitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(FormDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormDefinitionDto>> Get(
        Guid definitionId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetFormDefinitionByVersionQuery(definitionId, versionId),
            cancellationToken);

        return result is null ? NoContent() : Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(FormDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormDefinitionDto>> Configure(
        Guid definitionId,
        Guid versionId,
        [FromBody] ConfigureFormDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var fields = (request.Fields ?? Array.Empty<FormFieldRequest>())
            .Select(f => new FormFieldInputDto
            {
                FieldKey = f.FieldKey,
                Label = f.Label,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                SectionKey = f.SectionKey,
                HelpText = f.HelpText,
                Placeholder = f.Placeholder,
                DisplayText = f.DisplayText,
                Options = f.Options?.Select(o => new FieldOptionDto
                {
                    Value = o.Value,
                    Label = o.Label,
                }).ToList(),
            })
            .ToList();

        var result = await _mediator.Send(
            new ConfigureFormDefinitionCommand(definitionId, versionId, fields),
            cancellationToken);

        return Ok(result);
    }
}
