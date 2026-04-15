namespace Eap.Application.Policies.Models;

/// <summary>Metadata describing a document attached to a policy version.</summary>
public sealed class PolicyDocumentDto
{
    public Guid Id { get; init; }

    public Guid PolicyVersionId { get; init; }

    public string FileName { get; init; } = default!;

    public string ContentType { get; init; } = default!;

    public long FileSize { get; init; }

    public DateTimeOffset UploadedAtUtc { get; init; }
}
