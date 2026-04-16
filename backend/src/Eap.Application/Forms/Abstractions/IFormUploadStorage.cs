namespace Eap.Application.Forms.Abstractions;

/// <summary>
/// Storage abstraction for file uploads attached to form-based disclosure
/// submissions (BR-076). Mirrors the <c>IPolicyDocumentStorage</c> contract
/// but scoped to form uploads.
/// </summary>
public interface IFormUploadStorage
{
    /// <summary>Stores a file and returns a relative storage reference.</summary>
    Task<FormUploadStorageResult> StoreAsync(
        Guid submissionId,
        string fieldKey,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken);

    /// <summary>Opens a readable stream for a previously stored file.</summary>
    Task<Stream> OpenReadAsync(string storageReference, CancellationToken cancellationToken);

    /// <summary>Deletes the file at the given storage reference.</summary>
    Task DeleteAsync(string storageReference, CancellationToken cancellationToken);
}

/// <summary>Result of a successful file upload.</summary>
public sealed record FormUploadStorageResult(string StorageReference, long FileSize);
