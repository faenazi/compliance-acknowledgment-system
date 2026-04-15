using Eap.Domain.Policy;

namespace Eap.Application.Policies.Models;

/// <summary>Lightweight projection of a policy version used in list endpoints.</summary>
public sealed class PolicyVersionSummaryDto
{
    public Guid Id { get; init; }

    public Guid PolicyId { get; init; }

    public int VersionNumber { get; init; }

    public string? VersionLabel { get; init; }

    public PolicyVersionStatus Status { get; init; }

    public DateOnly? EffectiveDate { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; init; }

    public DateTimeOffset? ArchivedAtUtc { get; init; }

    public bool HasDocument { get; init; }
}

/// <summary>Full detail projection of a policy version, including audit fields and document metadata.</summary>
public sealed class PolicyVersionDetailDto
{
    public Guid Id { get; init; }

    public Guid PolicyId { get; init; }

    public int VersionNumber { get; init; }

    public string? VersionLabel { get; init; }

    public string? Summary { get; init; }

    public DateOnly? EffectiveDate { get; init; }

    public PolicyVersionStatus Status { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; init; }

    public string? PublishedBy { get; init; }

    public DateTimeOffset? ArchivedAtUtc { get; init; }

    public string? ArchivedBy { get; init; }

    public Guid? SupersededByPolicyVersionId { get; init; }

    public PolicyDocumentDto? Document { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedBy { get; init; }
}
