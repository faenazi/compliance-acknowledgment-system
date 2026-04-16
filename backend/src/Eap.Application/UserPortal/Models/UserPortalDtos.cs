using Eap.Application.Forms.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;

namespace Eap.Application.UserPortal.Models;

/// <summary>Dashboard summary for the current user (Sprint 6).</summary>
public sealed class MyDashboardDto
{
    public int PendingCount { get; init; }
    public int OverdueCount { get; init; }
    public int CompletedCount { get; init; }
    public IReadOnlyList<MyActionSummaryDto> PendingActions { get; init; } = [];
    public IReadOnlyList<MyActionSummaryDto> RecentlyCompleted { get; init; } = [];
}

/// <summary>Lightweight action summary used in lists and dashboard cards.</summary>
public sealed class MyActionSummaryDto
{
    public Guid RequirementId { get; init; }
    public string Title { get; init; } = default!;
    public string PolicyTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public RecurrenceModel RecurrenceModel { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public DateOnly? DueDate { get; init; }
    public UserActionRequirementStatus Status { get; init; }
    public DateTimeOffset AssignedAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
    public string CycleReference { get; init; } = default!;
}

/// <summary>Full detail for a single action the user needs to complete or has completed.</summary>
public sealed class MyActionDetailDto
{
    // Requirement
    public Guid RequirementId { get; init; }
    public UserActionRequirementStatus Status { get; init; }
    public DateOnly? DueDate { get; init; }
    public DateTimeOffset AssignedAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
    public string CycleReference { get; init; } = default!;

    // Acknowledgment
    public Guid AcknowledgmentVersionId { get; init; }
    public Guid AcknowledgmentDefinitionId { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public ActionType ActionType { get; init; }
    public RecurrenceModel RecurrenceModel { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public string? Summary { get; init; }
    public string? CommitmentText { get; init; }
    public DateOnly? StartDate { get; init; }

    // Policy
    public Guid PolicyVersionId { get; init; }
    public Guid PolicyId { get; init; }
    public string PolicyTitle { get; init; } = default!;
    public string? PolicySummary { get; init; }
    public int PolicyVersionNumber { get; init; }
    public string? PolicyVersionLabel { get; init; }
    public bool HasPolicyDocument { get; init; }

    // Form (if FormBasedDisclosure)
    public FormDefinitionDto? FormDefinition { get; init; }

    // Existing submission (if already completed)
    public Guid? SubmissionId { get; init; }
    public DateTimeOffset? SubmittedAtUtc { get; init; }
}

/// <summary>A single row in the user's submission history.</summary>
public sealed class MyHistoryItemDto
{
    public Guid SubmissionId { get; init; }
    public Guid RequirementId { get; init; }
    public string Title { get; init; } = default!;
    public string PolicyTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public int VersionNumber { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public bool IsLateSubmission { get; init; }
    public string CycleReference { get; init; } = default!;
}

/// <summary>Full detail of a past submission, read-only.</summary>
public sealed class MySubmissionDetailDto
{
    public Guid SubmissionId { get; init; }
    public Guid RequirementId { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public bool IsLateSubmission { get; init; }

    // Acknowledgment context
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public ActionType ActionType { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public int VersionNumber { get; init; }
    public string? CommitmentText { get; init; }

    // Policy context
    public string PolicyTitle { get; init; } = default!;
    public int PolicyVersionNumber { get; init; }
    public string? PolicyVersionLabel { get; init; }

    // Submitted data
    public string SubmissionJson { get; init; } = default!;
    public string? FormDefinitionSnapshotJson { get; init; }
    public IReadOnlyList<SubmissionFieldValueDto>? FieldValues { get; init; }
}

/// <summary>Result returned after a successful submission.</summary>
public sealed class SubmissionResultDto
{
    public Guid SubmissionId { get; init; }
    public Guid RequirementId { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public UserActionRequirementStatus RequirementStatus { get; init; }
    public bool IsLateSubmission { get; init; }
}
