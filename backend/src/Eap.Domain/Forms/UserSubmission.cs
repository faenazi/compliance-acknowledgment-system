using Eap.Domain.Common;

namespace Eap.Domain.Forms;

/// <summary>
/// A user's submission against an acknowledgment version — covers simple
/// acknowledgments, commitment acknowledgments, and form-based disclosures
/// (docs/05-data/conceptual-data-model.md §8.1, BR-078, BR-091).
///
/// For form-based disclosures the <see cref="FormDefinitionId"/> and
/// <see cref="FormDefinitionSnapshotJson"/> capture the form at submission
/// time (BR-079 / CDM-004). For simple and commitment acknowledgments
/// these are null.
/// </summary>
public sealed class UserSubmission : AuditableEntity
{
    private readonly List<UserSubmissionFieldValue> _fieldValues = new();

    // EF Core
    private UserSubmission() { }

    /// <summary>
    /// Constructor for form-based disclosure submissions (Sprint 5).
    /// </summary>
    public UserSubmission(
        Guid userId,
        Guid acknowledgmentVersionId,
        Guid formDefinitionId,
        string submissionJson,
        string formDefinitionSnapshotJson,
        string? submittedBy)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));
        if (acknowledgmentVersionId == Guid.Empty)
            throw new ArgumentException("Acknowledgment version id is required.", nameof(acknowledgmentVersionId));
        if (formDefinitionId == Guid.Empty)
            throw new ArgumentException("Form definition id is required.", nameof(formDefinitionId));
        if (string.IsNullOrWhiteSpace(submissionJson))
            throw new ArgumentException("Submission data is required.", nameof(submissionJson));
        if (string.IsNullOrWhiteSpace(formDefinitionSnapshotJson))
            throw new ArgumentException("Form definition snapshot is required (BR-079).", nameof(formDefinitionSnapshotJson));

        UserId = userId;
        AcknowledgmentVersionId = acknowledgmentVersionId;
        FormDefinitionId = formDefinitionId;
        SubmissionJson = submissionJson;
        FormDefinitionSnapshotJson = formDefinitionSnapshotJson;
        Status = SubmissionStatus.Submitted;
        SubmittedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = submittedBy?.Trim();
    }

    /// <summary>
    /// Constructor for simple and commitment acknowledgment submissions (Sprint 6).
    /// No form definition required — <see cref="SubmissionJson"/> contains
    /// a minimal confirmation payload.
    /// </summary>
    public UserSubmission(
        Guid userId,
        Guid acknowledgmentVersionId,
        Guid? userActionRequirementId,
        string submissionJson,
        bool isLateSubmission,
        string? submittedBy)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));
        if (acknowledgmentVersionId == Guid.Empty)
            throw new ArgumentException("Acknowledgment version id is required.", nameof(acknowledgmentVersionId));
        if (string.IsNullOrWhiteSpace(submissionJson))
            throw new ArgumentException("Submission data is required.", nameof(submissionJson));

        UserId = userId;
        AcknowledgmentVersionId = acknowledgmentVersionId;
        UserActionRequirementId = userActionRequirementId;
        SubmissionJson = submissionJson;
        IsLateSubmission = isLateSubmission;
        Status = SubmissionStatus.Submitted;
        SubmittedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = submittedBy?.Trim();
    }

    public Guid UserId { get; private set; }

    public Guid AcknowledgmentVersionId { get; private set; }

    /// <summary>Nullable — null for Sprint 5 legacy form submissions without a linked requirement.</summary>
    public Guid? FormDefinitionId { get; private set; }

    /// <summary>Links this submission to the requirement that prompted it (Sprint 6).
    /// Nullable to preserve backward compatibility with Sprint 5 form submissions.</summary>
    public Guid? UserActionRequirementId { get; private set; }

    /// <summary>JSON payload containing the user's field answers or acknowledgment confirmation.</summary>
    public string SubmissionJson { get; private set; } = default!;

    /// <summary>JSON snapshot of the form definition at submission time (BR-079). Null for non-form submissions.</summary>
    public string? FormDefinitionSnapshotJson { get; private set; }

    public SubmissionStatus Status { get; private set; }

    public DateTimeOffset SubmittedAtUtc { get; private set; }

    /// <summary>True when the submission was made after the requirement's due date (BR-103).</summary>
    public bool IsLateSubmission { get; private set; }

    /// <summary>Optional flattened field values for reporting (§8.2).</summary>
    public IReadOnlyCollection<UserSubmissionFieldValue> FieldValues => _fieldValues.AsReadOnly();

    /// <summary>Sets the linked requirement id and late-submission flag (Sprint 6 enrichment).</summary>
    public void LinkToRequirement(Guid requirementId, bool isLate)
    {
        UserActionRequirementId = requirementId;
        IsLateSubmission = isLate;
    }

    /// <summary>Adds a flattened field value for reporting purposes (optional, §8.2).</summary>
    public void AddFieldValue(UserSubmissionFieldValue fieldValue)
    {
        ArgumentNullException.ThrowIfNull(fieldValue);
        _fieldValues.Add(fieldValue);
    }
}
