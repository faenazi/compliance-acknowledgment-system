using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;

namespace Eap.Application.Admin.Models;

/// <summary>Enriched requirement row for the admin monitoring table (Sprint 7).</summary>
public sealed class AdminRequirementSummaryDto
{
    public Guid RequirementId { get; init; }

    // User context
    public Guid UserId { get; init; }
    public string UserDisplayName { get; init; } = default!;
    public string UserDepartment { get; init; } = default!;

    // Action context
    public Guid AcknowledgmentDefinitionId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public string ActionTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public RecurrenceModel RecurrenceModel { get; init; }
    public int VersionNumber { get; init; }

    // Policy context
    public string PolicyTitle { get; init; } = default!;

    // Requirement status
    public UserActionRequirementStatus Status { get; init; }
    public string CycleReference { get; init; } = default!;
    public DateTimeOffset AssignedAtUtc { get; init; }
    public DateOnly? DueDate { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
    public bool IsCurrent { get; init; }
}

/// <summary>Full detail of a single requirement visible to admin (Sprint 7).</summary>
public sealed class AdminRequirementDetailDto
{
    // Requirement
    public Guid RequirementId { get; init; }
    public UserActionRequirementStatus Status { get; init; }
    public DateOnly? DueDate { get; init; }
    public DateTimeOffset AssignedAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
    public string CycleReference { get; init; } = default!;
    public bool IsCurrent { get; init; }

    // User
    public Guid UserId { get; init; }
    public string UserDisplayName { get; init; } = default!;
    public string UserDepartment { get; init; } = default!;
    public string? UserEmail { get; init; }

    // Acknowledgment
    public Guid AcknowledgmentDefinitionId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public string ActionTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public RecurrenceModel RecurrenceModel { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public int VersionNumber { get; init; }

    // Policy
    public Guid PolicyId { get; init; }
    public Guid PolicyVersionId { get; init; }
    public string PolicyTitle { get; init; } = default!;
    public int PolicyVersionNumber { get; init; }

    // Submission if any
    public Guid? SubmissionId { get; init; }
    public DateTimeOffset? SubmittedAtUtc { get; init; }
    public bool? IsLateSubmission { get; init; }
}

/// <summary>Full detail of a submission visible to admin (Sprint 7).</summary>
public sealed class AdminSubmissionDetailDto
{
    public Guid SubmissionId { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public bool IsLateSubmission { get; init; }

    // User context
    public Guid UserId { get; init; }
    public string UserDisplayName { get; init; } = default!;
    public string UserDepartment { get; init; } = default!;

    // Acknowledgment context
    public Guid AcknowledgmentDefinitionId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public string ActionTitle { get; init; } = default!;
    public string? ActionDescription { get; init; }
    public ActionType ActionType { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public int VersionNumber { get; init; }
    public string? CommitmentText { get; init; }

    // Policy context
    public string PolicyTitle { get; init; } = default!;
    public int PolicyVersionNumber { get; init; }
    public string? PolicyVersionLabel { get; init; }

    // Requirement context
    public Guid? RequirementId { get; init; }
    public string? CycleReference { get; init; }

    // Submitted data
    public string SubmissionJson { get; init; } = default!;
    public string? FormDefinitionSnapshotJson { get; init; }
    public IReadOnlyList<AdminFieldValueDto>? FieldValues { get; init; }
}

/// <summary>Flattened field value for admin review.</summary>
public sealed class AdminFieldValueDto
{
    public Guid Id { get; init; }
    public string FieldKey { get; init; } = default!;
    public string FieldLabel { get; init; } = default!;
    public string FieldType { get; init; } = default!;
    public string? ValueText { get; init; }
    public decimal? ValueNumber { get; init; }
    public DateOnly? ValueDate { get; init; }
    public bool? ValueBoolean { get; init; }
    public string? ValueJson { get; init; }
}

/// <summary>Filter parameters for the admin requirement monitoring list.</summary>
public sealed record AdminRequirementsFilter(
    int Page,
    int PageSize,
    UserActionRequirementStatus? Status = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    string? Department = null,
    RecurrenceModel? RecurrenceModel = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    string? Search = null,
    bool CurrentOnly = true);
