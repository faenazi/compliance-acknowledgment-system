using Eap.Application.Forms.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Forms;

/// <summary>
/// File upload/download endpoints for form-based disclosure fields (BR-076).
/// </summary>
[ApiController]
[Route("api/form-uploads")]
[Authorize]
public sealed class FormUploadsController : ControllerBase
{
    private readonly IFormUploadStorage _storage;

    public FormUploadsController(IFormUploadStorage storage)
    {
        _storage = storage;
    }

    [HttpPost("{submissionId:guid}/{fieldKey}")]
    [ProducesResponseType(typeof(FormUploadResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<FormUploadResult>> Upload(
        Guid submissionId,
        string fieldKey,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { title = "A file is required." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _storage.StoreAsync(
            submissionId, fieldKey, file.FileName, file.ContentType, stream, cancellationToken);

        return Ok(new FormUploadResult(result.StorageReference, result.FileSize, file.FileName));
    }

    [HttpGet("{**storageReference}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        string storageReference,
        CancellationToken cancellationToken)
    {
        var stream = await _storage.OpenReadAsync(storageReference, cancellationToken);
        return File(stream, "application/octet-stream");
    }
}

public sealed record FormUploadResult(string StorageReference, long FileSize, string FileName);
