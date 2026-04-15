namespace Eap.Application.Policies.Abstractions;

/// <summary>
/// Storage abstraction for uploaded policy documents. The default MVP
/// implementation writes to a configurable file-system root; the contract
/// is intentionally generic so the storage backend can change (object
/// storage, ECM) without touching the domain or application layers.
/// </summary>
public interface IPolicyDocumentStorage
{
    /// <summary>
    /// Persists the supplied content and returns an opaque storage reference
    /// that the domain entity stores alongside the document metadata.
    /// </summary>
    Task<PolicyDocumentStorageResult> StoreAsync(
        Guid policyId,
        Guid policyVersionId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken);

    /// <summary>Opens a read stream for the document at <paramref name="storageReference"/>.</summary>
    Task<Stream> OpenReadAsync(string storageReference, CancellationToken cancellationToken);

    /// <summary>Deletes the underlying artifact. Safe to call if already removed.</summary>
    Task DeleteAsync(string storageReference, CancellationToken cancellationToken);
}

/// <summary>
/// Result of a successful <see cref="IPolicyDocumentStorage.StoreAsync"/> call.
/// <paramref name="StorageReference"/> is opaque to callers.
/// </summary>
public sealed record PolicyDocumentStorageResult(string StorageReference, long FileSize);
