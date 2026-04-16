using Eap.Application.Forms.Commands.SubmitForm;
using Eap.Application.Forms.Models;
using Eap.Application.Forms.Queries.GetSubmissionById;
using Eap.Application.Forms.Queries.ListSubmissionsByVersion;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Forms;

/// <summary>
/// Form submission endpoints (Sprint 5). Submission is open to any authenticated
/// user; listing/detail is restricted to admin roles (Acknowledgment Manager,
/// Compliance Viewer, Auditor, System Administrator).
/// </summary>
[ApiController]
[Authorize]
public sealed class FormSubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FormSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("api/acknowledgments/{definitionId:guid}/versions/{versionId:guid}/submissions")]
    [ProducesResponseType(typeof(UserSubmissionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserSubmissionDetailDto>> Submit(
        Guid definitionId,
        Guid versionId,
        [FromBody] SubmitFormRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SubmitFormCommand(definitionId, versionId, request.SubmissionJson),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("api/acknowledgments/{definitionId:guid}/versions/{versionId:guid}/submissions")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + ","
                     + SystemRoles.ComplianceViewer + ","
                     + SystemRoles.Auditor + ","
                     + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(ListSubmissionsByVersionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListSubmissionsByVersionResult>> List(
        Guid definitionId,
        Guid versionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListSubmissionsByVersionQuery(definitionId, versionId, page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("api/submissions/{submissionId:guid}")]
    [Authorize(Roles = SystemRoles.AcknowledgmentManager + ","
                     + SystemRoles.ComplianceViewer + ","
                     + SystemRoles.Auditor + ","
                     + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(UserSubmissionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserSubmissionDetailDto>> GetById(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubmissionByIdQuery(submissionId),
            cancellationToken);

        return Ok(result);
    }
}
