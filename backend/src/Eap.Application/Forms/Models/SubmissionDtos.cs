using Eap.Domain.Forms;

namespace Eap.Application.Forms.Models;

/// <summary>DTO for a flattened field value within a submission.</summary>
public sealed class SubmissionFieldValueDto
{
    public Guid Id { get; init; }
    public string FieldKey { get; init; } = default!;
    public string FieldLabel { get; init; } = default!;
    public FormFieldType FieldType { get; init; }
    public string? ValueText { get; init; }
    public decimal? ValueNumber { get; init; }
    public DateOnly? ValueDate { get; init; }
    public bool? ValueBoolean { get; init; }
    public string? ValueJson { get; init; }
}

/// <summary>Summary DTO for a user submission (list views).</summary>
public sealed class UserSubmissionSummaryDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public Guid FormDefinitionId { get; init; }
    public SubmissionStatus Status { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public string? CreatedBy { get; init; }
}

/// <summary>Detail DTO for a user submission (includes payload + snapshot).</summary>
public sealed class UserSubmissionDetailDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public Guid FormDefinitionId { get; init; }
    public string SubmissionJson { get; init; } = default!;
    public string FormDefinitionSnapshotJson { get; init; } = default!;
    public SubmissionStatus Status { get; init; }
    public DateTimeOffset SubmittedAtUtc { get; init; }
    public IReadOnlyList<SubmissionFieldValueDto> FieldValues { get; init; } = Array.Empty<SubmissionFieldValueDto>();
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
