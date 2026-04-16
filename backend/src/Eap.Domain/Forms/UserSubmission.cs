using Eap.Domain.Common;

namespace Eap.Domain.Forms;

/// <summary>
/// A user's submission against a form-based disclosure version
/// (docs/05-data/conceptual-data-model.md §8.1, BR-078, BR-091).
///
/// Stores both the structured <see cref="SubmissionJson"/> (the user's answers)
/// and a <see cref="FormDefinitionSnapshotJson"/> (the form as it existed at
/// submission time, BR-079 / CDM-004).
/// </summary>
public sealed class UserSubmission : AuditableEntity
{
    private readonly List<UserSubmissionFieldValue> _fieldValues = new();

    // EF Core
    private UserSubmission() { }

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

    public Guid UserId { get; private set; }

    public Guid AcknowledgmentVersionId { get; private set; }

    public Guid FormDefinitionId { get; private set; }

    /// <summary>JSON payload containing the user's field answers.</summary>
    public string SubmissionJson { get; private set; } = default!;

    /// <summary>JSON snapshot of the form definition at submission time (BR-079).</summary>
    public string FormDefinitionSnapshotJson { get; private set; } = default!;

    public SubmissionStatus Status { get; private set; }

    public DateTimeOffset SubmittedAtUtc { get; private set; }

    /// <summary>Optional flattened field values for reporting (§8.2).</summary>
    public IReadOnlyCollection<UserSubmissionFieldValue> FieldValues => _fieldValues.AsReadOnly();

    /// <summary>Adds a flattened field value for reporting purposes (optional, §8.2).</summary>
    public void AddFieldValue(UserSubmissionFieldValue fieldValue)
    {
        ArgumentNullException.ThrowIfNull(fieldValue);
        _fieldValues.Add(fieldValue);
    }
}
