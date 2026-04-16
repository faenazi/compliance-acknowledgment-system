namespace Eap.Domain.Forms;

/// <summary>
/// Supported Phase 1 field types for form-based disclosures (BR-073).
/// Numeric values are part of the public API contract — never reorder.
/// </summary>
public enum FormFieldType
{
    ShortText = 0,
    LongText = 1,
    Number = 2,
    Decimal = 3,
    Date = 4,
    Checkbox = 5,
    RadioGroup = 6,
    Dropdown = 7,
    MultiSelect = 8,
    YesNo = 9,
    Email = 10,
    PhoneNumber = 11,
    FileUpload = 12,
    ReadOnlyDisplay = 13,
    SectionHeader = 14,
}

/// <summary>Helpers describing the role of a <see cref="FormFieldType"/>.</summary>
public static class FormFieldTypes
{
    /// <summary>Field types whose options list (label/value pairs) is required.</summary>
    public static bool RequiresOptions(FormFieldType type) => type switch
    {
        FormFieldType.RadioGroup => true,
        FormFieldType.Dropdown => true,
        FormFieldType.MultiSelect => true,
        _ => false,
    };

    /// <summary>Display-only field types — they never collect a value.</summary>
    public static bool IsDisplayOnly(FormFieldType type) => type switch
    {
        FormFieldType.ReadOnlyDisplay => true,
        FormFieldType.SectionHeader => true,
        _ => false,
    };

    /// <summary>True when the field collects a user-submitted value (not display-only).</summary>
    public static bool CollectsValue(FormFieldType type) => !IsDisplayOnly(type);
}
