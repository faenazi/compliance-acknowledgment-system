using Eap.Domain.Common;

namespace Eap.Domain.Forms;

/// <summary>
/// Version-bound form definition attached 0..1 to an <c>AcknowledgmentVersion</c>
/// (docs/05-data/conceptual-data-model.md §6.3, BR-070..BR-072).
///
/// The definition stores the complete set of <see cref="FormField"/> entries.
/// Fields are replaced wholesale on each save — individual field CRUD is handled
/// at the aggregate level via <see cref="ReplaceFields"/>.
/// </summary>
public sealed class FormDefinition : AuditableEntity
{
    private readonly List<FormField> _fields = new();

    // EF Core
    private FormDefinition() { }

    internal FormDefinition(Guid acknowledgmentVersionId, string? createdBy)
    {
        if (acknowledgmentVersionId == Guid.Empty)
            throw new ArgumentException(
                "Acknowledgment version id is required.", nameof(acknowledgmentVersionId));

        AcknowledgmentVersionId = acknowledgmentVersionId;
        SchemaVersion = 1;
        IsActive = true;
        CreatedBy = createdBy?.Trim();
    }

    public Guid AcknowledgmentVersionId { get; private set; }

    /// <summary>Schema version identifier for forward compatibility.</summary>
    public int SchemaVersion { get; private set; }

    /// <summary>Whether this definition is the active one for the owning version.</summary>
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<FormField> Fields => _fields.AsReadOnly();

    /// <summary>True when at least one field that collects user input exists —
    /// required before publishing a FormBasedDisclosure version (BR-070).</summary>
    public bool HasAnyInputField =>
        _fields.Any(f => FormFieldTypes.CollectsValue(f.FieldType));

    /// <summary>
    /// Replaces all fields with the supplied set. Validates uniqueness of field
    /// keys and option requirements (BR-075).
    /// </summary>
    public void ReplaceFields(IEnumerable<FormFieldInput> fieldInputs, string? updatedBy)
    {
        ArgumentNullException.ThrowIfNull(fieldInputs);

        var inputs = fieldInputs.ToList();
        ValidateFieldKeys(inputs);

        _fields.Clear();
        var index = 0;
        foreach (var input in inputs)
        {
            _fields.Add(new FormField(
                formDefinitionId: Id,
                fieldKey: input.FieldKey,
                label: input.Label,
                fieldType: input.FieldType,
                isRequired: input.IsRequired,
                sortOrder: index++,
                sectionKey: input.SectionKey,
                helpText: input.HelpText,
                placeholder: input.Placeholder,
                displayText: input.DisplayText,
                options: input.Options,
                createdBy: updatedBy));
        }

        Touch(updatedBy);
    }

    /// <summary>Produces a JSON-serializable snapshot of this definition for storage
    /// alongside a <see cref="UserSubmission"/> (BR-079 / CDM-004).</summary>
    public FormDefinitionSnapshot TakeSnapshot() => new(
        Id,
        AcknowledgmentVersionId,
        SchemaVersion,
        _fields.OrderBy(f => f.SortOrder)
            .Select(f => new FormFieldSnapshot(
                f.FieldKey,
                f.Label,
                f.FieldType,
                f.IsRequired,
                f.SortOrder,
                f.SectionKey,
                f.HelpText,
                f.Placeholder,
                f.DisplayText,
                f.Options.ToList()))
            .ToList());

    private static void ValidateFieldKeys(List<FormFieldInput> inputs)
    {
        var keys = inputs
            .Where(f => !FormFieldTypes.IsDisplayOnly(f.FieldType))
            .Select(f => f.FieldKey)
            .ToList();

        var duplicates = keys
            .GroupBy(k => k, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new ArgumentException(
                $"Duplicate field keys: {string.Join(", ", duplicates)}. Each field key must be unique within a form definition.");
    }

    private void Touch(string? updatedBy)
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }
}

/// <summary>
/// Input shape used to batch-replace fields on a <see cref="FormDefinition"/>.
/// Kept in the domain namespace so both application and domain layers can reference it.
/// </summary>
public sealed record FormFieldInput(
    string FieldKey,
    string Label,
    FormFieldType FieldType,
    bool IsRequired,
    string? SectionKey,
    string? HelpText,
    string? Placeholder,
    string? DisplayText,
    IReadOnlyList<FieldOption>? Options);

/// <summary>Immutable snapshot of a form definition at submission time (BR-079).</summary>
public sealed record FormDefinitionSnapshot(
    Guid FormDefinitionId,
    Guid AcknowledgmentVersionId,
    int SchemaVersion,
    IReadOnlyList<FormFieldSnapshot> Fields);

/// <summary>Snapshot of a single field within a <see cref="FormDefinitionSnapshot"/>.</summary>
public sealed record FormFieldSnapshot(
    string FieldKey,
    string Label,
    FormFieldType FieldType,
    bool IsRequired,
    int SortOrder,
    string? SectionKey,
    string? HelpText,
    string? Placeholder,
    string? DisplayText,
    IReadOnlyList<FieldOption> Options);
