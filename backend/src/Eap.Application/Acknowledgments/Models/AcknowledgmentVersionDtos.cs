using Eap.Domain.Acknowledgment;

namespace Eap.Application.Acknowledgments.Models;

/// <summary>Lightweight projection of an acknowledgment version used in list endpoints.</summary>
public sealed class AcknowledgmentVersionSummaryDto
{
    public Guid Id { get; init; }

    public Guid AcknowledgmentDefinitionId { get; init; }

    public int VersionNumber { get; init; }

    public string? VersionLabel { get; init; }

    public Guid PolicyVersionId { get; init; }

    public ActionType ActionType { get; init; }

    public AcknowledgmentVersionStatus Status { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? DueDate { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; init; }

    public DateTimeOffset? ArchivedAtUtc { get; init; }
}

/// <summary>Full detail projection of an acknowledgment version.</summary>
public sealed class AcknowledgmentVersionDetailDto
{
    public Guid Id { get; init; }

    public Guid AcknowledgmentDefinitionId { get; init; }

    public int VersionNumber { get; init; }

    public string? VersionLabel { get; init; }

    public Guid PolicyVersionId { get; init; }

    public ActionType ActionType { get; init; }

    public string? Summary { get; init; }

    public string? CommitmentText { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? DueDate { get; init; }

    public AcknowledgmentVersionStatus Status { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; init; }

    public string? PublishedBy { get; init; }

    public DateTimeOffset? ArchivedAtUtc { get; init; }

    public string? ArchivedBy { get; init; }

    public Guid? SupersededByAcknowledgmentVersionId { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedBy { get; init; }
}
