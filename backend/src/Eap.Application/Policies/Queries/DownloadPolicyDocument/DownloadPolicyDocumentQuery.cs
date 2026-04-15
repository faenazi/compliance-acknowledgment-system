using MediatR;

namespace Eap.Application.Policies.Queries.DownloadPolicyDocument;

/// <summary>
/// Resolves the storage-backed stream for a policy version's document.
/// The returned stream is owned by the caller and MUST be disposed.
/// </summary>
public sealed record DownloadPolicyDocumentQuery(Guid PolicyId, Guid VersionId)
    : IRequest<PolicyDocumentDownloadResult>;

public sealed class PolicyDocumentDownloadResult
{
    public required Stream Content { get; init; }

    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public required long FileSize { get; init; }
}
