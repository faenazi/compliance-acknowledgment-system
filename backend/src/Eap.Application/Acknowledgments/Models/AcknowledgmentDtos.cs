using Eap.Domain.Acknowledgment;

namespace Eap.Application.Acknowledgments.Models;

/// <summary>Projection for acknowledgment list endpoints.</summary>
public sealed class AcknowledgmentDefinitionSummaryDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public string OwnerDepartment { get; init; } = default!;

    public ActionType DefaultActionType { get; init; }

    public AcknowledgmentStatus Status { get; init; }

    public Guid? CurrentAcknowledgmentVersionId { get; init; }

    public int? CurrentVersionNumber { get; init; }

    public int VersionsCount { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}

/// <summary>Full acknowledgment definition record with version history.</summary>
public sealed class AcknowledgmentDefinitionDetailDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public string OwnerDepartment { get; init; } = default!;

    public ActionType DefaultActionType { get; init; }

    public string? Description { get; init; }

    public AcknowledgmentStatus Status { get; init; }

    public Guid? CurrentAcknowledgmentVersionId { get; init; }

    public IReadOnlyList<AcknowledgmentVersionSummaryDto> Versions { get; init; }
        = Array.Empty<AcknowledgmentVersionSummaryDto>();

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedBy { get; init; }
}
