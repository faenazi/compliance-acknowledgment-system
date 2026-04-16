using Eap.Domain.Common;

namespace Eap.Domain.Forms;

/// <summary>
/// A single field inside a <see cref="FormDefinition"/> (BR-073).
/// Fields belong to a section and are ordered within it by <see cref="SortOrder"/>.
/// Display-only field types (<see cref="FormFieldType.ReadOnlyDisplay"/>,
/// <see cref="FormFieldType.SectionHeader"/>) never collect a value.
/// </summary>
public sealed class FormField : Entity
{
    // EF Core
    private FormField() { }

    internal FormField(
        Guid formDefinitionId,
        string fieldKey,
        string label,
        FormFieldType fieldType,
        bool isRequired,
        int sortOrder,
        string? sectionKey,
        string? helpText,
        string? placeholder,
        string? displayText,
        IReadOnlyList<FieldOption>? options,
        string? createdBy)
    {
        if (formDefinitionId == Guid.Empty)
            throw new ArgumentException("Form definition id is required.", nameof(formDefinitionId));

        if (string.IsNullOrWhiteSpace(fieldKey))
            throw new ArgumentException("Field key is required.", nameof(fieldKey));

        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Field label is required.", nameof(label));

        if (FormFieldTypes.RequiresOptions(fieldType) && (options is null || options.Count == 0))
            throw new ArgumentException(
                $"Options are required for field type {fieldType} (BR-075).", nameof(options));

        FormDefinitionId = formDefinitionId;
        FieldKey = fieldKey.Trim();
        Label = label.Trim();
        FieldType = fieldType;
        IsRequired = FormFieldTypes.IsDisplayOnly(fieldType) ? false : isRequired;
        SortOrder = sortOrder;
        SectionKey = sectionKey?.Trim();
        HelpText = helpText?.Trim();
        Placeholder = placeholder?.Trim();
        DisplayText = displayText?.Trim();
        Options = options ?? Array.Empty<FieldOption>();
        CreatedBy = createdBy?.Trim();
    }

    public Guid FormDefinitionId { get; private set; }

    /// <summary>Machine-readable key used to identify this field in submission JSON.</summary>
    public string FieldKey { get; private set; } = default!;

    /// <summary>Human-readable label displayed to the user.</summary>
    public string Label { get; private set; } = default!;

    public FormFieldType FieldType { get; private set; }

    /// <summary>When true the field must have a non-empty value before submission (BR-074).
    /// Always false for display-only field types.</summary>
    public bool IsRequired { get; private set; }

    /// <summary>Ordering within the section (or at root if <see cref="SectionKey"/> is null).</summary>
    public int SortOrder { get; private set; }

    /// <summary>Optional grouping key linking this field to a named section.</summary>
    public string? SectionKey { get; private set; }

    /// <summary>Optional help / guidance text shown below the label.</summary>
    public string? HelpText { get; private set; }

    /// <summary>Optional placeholder shown inside input controls.</summary>
    public string? Placeholder { get; private set; }

    /// <summary>Content rendered for <see cref="FormFieldType.ReadOnlyDisplay"/> or
    /// <see cref="FormFieldType.SectionHeader"/> fields.</summary>
    public string? DisplayText { get; private set; }

    /// <summary>Ordered options for select/radio/multi-select fields (BR-075).</summary>
    public IReadOnlyList<FieldOption> Options { get; private set; } = Array.Empty<FieldOption>();

    /// <summary>Who created this field.</summary>
    public string? CreatedBy { get; private set; }
}
