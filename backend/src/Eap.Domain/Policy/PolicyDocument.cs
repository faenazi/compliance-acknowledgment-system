using Eap.Domain.Common;

namespace Eap.Domain.Policy;

/// <summary>
/// Uploaded policy document attached to a specific <see cref="PolicyVersion"/>
/// (docs/05-data/conceptual-data-model.md §5.3). For MVP only a single primary
/// PDF is expected per version but the model is kept explicit in case operational
/// replacements (before publishing) need to be tracked.
/// </summary>
public sealed class PolicyDocument : AuditableEntity
{
    // EF Core
    private PolicyDocument() { }

    public PolicyDocument(
        Guid policyVersionId,
        string fileName,
        string contentType,
        long fileSize,
        string storageReference,
        string? uploadedBy)
    {
        if (policyVersionId == Guid.Empty)
        {
            throw new ArgumentException("Policy version id is required.", nameof(policyVersionId));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(storageReference))
        {
            throw new ArgumentException("Storage reference is required.", nameof(storageReference));
        }

        if (fileSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fileSize), "File size must be positive.");
        }

        PolicyVersionId = policyVersionId;
        FileName = fileName.Trim();
        ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType.Trim();
        FileSize = fileSize;
        StorageReference = storageReference.Trim();
        UploadedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = uploadedBy?.Trim();
    }

    public Guid PolicyVersionId { get; private set; }

    public string FileName { get; private set; } = default!;

    public string ContentType { get; private set; } = default!;

    public long FileSize { get; private set; }

    /// <summary>
    /// Opaque pointer into the configured document store (e.g. a relative file
    /// path under the upload root). Storage backend details are encapsulated
    /// in <c>IPolicyDocumentStorage</c>; this field remains backend-neutral.
    /// </summary>
    public string StorageReference { get; private set; } = default!;

    public DateTimeOffset UploadedAtUtc { get; private set; }
}
