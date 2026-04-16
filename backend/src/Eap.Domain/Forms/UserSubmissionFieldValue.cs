using Eap.Domain.Common;

namespace Eap.Domain.Forms;

/// <summary>
/// Optional flattened storage of an individual submitted field value for
/// query/reporting purposes (docs/05-data/conceptual-data-model.md §8.2).
/// </summary>
public sealed class UserSubmissionFieldValue : Entity
{
    // EF Core
    private UserSubmissionFieldValue() { }

    public UserSubmissionFieldValue(
        Guid userSubmissionId,
        string fieldKey,
        string fieldLabel,
        FormFieldType fieldType,
        string? valueText,
        decimal? valueNumber,
        DateOnly? valueDate,
        bool? valueBoolean,
        string? valueJson)
    {
        if (userSubmissionId == Guid.Empty)
            throw new ArgumentException("Submission id is required.", nameof(userSubmissionId));
        if (string.IsNullOrWhiteSpace(fieldKey))
            throw new ArgumentException("Field key is required.", nameof(fieldKey));

        UserSubmissionId = userSubmissionId;
        FieldKey = fieldKey.Trim();
        FieldLabel = fieldLabel?.Trim() ?? fieldKey.Trim();
        FieldType = fieldType;
        ValueText = valueText;
        ValueNumber = valueNumber;
        ValueDate = valueDate;
        ValueBoolean = valueBoolean;
        ValueJson = valueJson;
    }

    public Guid UserSubmissionId { get; private set; }

    public string FieldKey { get; private set; } = default!;

    public string FieldLabel { get; private set; } = default!;

    public FormFieldType FieldType { get; private set; }

    public string? ValueText { get; private set; }

    public decimal? ValueNumber { get; private set; }

    public DateOnly? ValueDate { get; private set; }

    public bool? ValueBoolean { get; private set; }

    /// <summary>JSON-encoded value for complex types (multi-select, etc.).</summary>
    public string? ValueJson { get; private set; }
}
