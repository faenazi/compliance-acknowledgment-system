using Eap.Application.Policies.Commands.UploadPolicyDocument;
using Eap.Application.Policies.Models;
using Eap.Application.Policies.Queries.DownloadPolicyDocument;
using Eap.Domain.Identity;
using Eap.Infrastructure.Policies.Documents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eap.Api.Features.Policies;

/// <summary>
/// Document attach/download for policy versions. Upload is restricted to draft
/// versions (enforced in the domain aggregate). Storage is pluggable — the
/// controller only deals with <c>IFormFile</c> → MediatR command and never
/// touches the storage backend directly.
/// </summary>
[ApiController]
[Route("api/policies/{policyId:guid}/versions/{versionId:guid}/document")]
[Authorize]
public sealed class PolicyDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PolicyDocumentStorageOptions _storageOptions;

    public PolicyDocumentsController(
        IMediator mediator,
        IOptions<PolicyDocumentStorageOptions> storageOptions)
    {
        _mediator = mediator;
        _storageOptions = storageOptions.Value;
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyDocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<ActionResult<PolicyDocumentDto>> Upload(
        Guid policyId,
        Guid versionId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length <= 0)
        {
            return BadRequest(new { detail = "An uploaded file is required." });
        }

        if (file.Length > _storageOptions.MaxFileSizeBytes)
        {
            return StatusCode(
                StatusCodes.Status413PayloadTooLarge,
                new { detail = $"File exceeds configured maximum of {_storageOptions.MaxFileSizeBytes} bytes." });
        }

        await using var stream = file.OpenReadStream();

        var result = await _mediator.Send(
            new UploadPolicyDocumentCommand(
                PolicyId: policyId,
                VersionId: versionId,
                FileName: file.FileName,
                ContentType: string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                FileSize: file.Length,
                Content: stream),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        Guid policyId,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DownloadPolicyDocumentQuery(policyId, versionId),
            cancellationToken);

        // FileStreamResult disposes the stream after the response is written.
        return new FileStreamResult(result.Content, result.ContentType)
        {
            FileDownloadName = result.FileName,
            EnableRangeProcessing = true,
        };
    }
}
