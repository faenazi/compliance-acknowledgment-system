using Eap.Domain.Policy;

namespace Eap.Application.Policies.Models;

/// <summary>Projection for policy list endpoints.</summary>
public sealed class PolicySummaryDto
{
    public Guid Id { get; init; }

    public string PolicyCode { get; init; } = default!;

    public string Title { get; init; } = default!;

    public string OwnerDepartment { get; init; } = default!;

    public string? Category { get; init; }

    public PolicyStatus Status { get; init; }

    public Guid? CurrentPolicyVersionId { get; init; }

    public int? CurrentVersionNumber { get; init; }

    public DateOnly? CurrentEffectiveDate { get; init; }

    public int VersionsCount { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}

/// <summary>Full policy record including version history for detail/edit screens.</summary>
public sealed class PolicyDetailDto
{
    public Guid Id { get; init; }

    public string PolicyCode { get; init; } = default!;

    public string Title { get; init; } = default!;

    public string OwnerDepartment { get; init; } = default!;

    public string? Category { get; init; }

    public string? Description { get; init; }

    public PolicyStatus Status { get; init; }

    public Guid? CurrentPolicyVersionId { get; init; }

    public IReadOnlyList<PolicyVersionSummaryDto> Versions { get; init; } = Array.Empty<PolicyVersionSummaryDto>();

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedBy { get; init; }
}
