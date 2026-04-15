using Eap.Domain.Requirements;

namespace Eap.Application.Requirements.Models;

/// <summary>DTO for a single scheduled action for a given user and version cycle.</summary>
public sealed class UserActionRequirementDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public Guid AcknowledgmentVersionId { get; init; }

    public string CycleReference { get; init; } = default!;

    public DateOnly? RecurrenceInstanceDate { get; init; }

    public DateOnly? DueDate { get; init; }

    public DateTimeOffset AssignedAtUtc { get; init; }

    public DateTimeOffset? CompletedAtUtc { get; init; }

    public UserActionRequirementStatus Status { get; init; }

    public bool IsCurrent { get; init; }
}

/// <summary>Summary of a generation pass — counts of newly created / skipped rows.</summary>
public sealed class RequirementGenerationSummaryDto
{
    public Guid AcknowledgmentVersionId { get; init; }

    public string CycleReference { get; init; } = default!;

    public int CreatedCount { get; init; }

    public int SkippedCount { get; init; }

    public int CancelledCount { get; init; }

    public DateTimeOffset GeneratedAtUtc { get; init; }
}
