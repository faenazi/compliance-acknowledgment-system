using System.Text.Json;
using System.Text.RegularExpressions;
using Eap.Domain.Forms;

namespace Eap.Application.Forms.Services;

/// <summary>
/// Validates a JSON submission payload against a <see cref="FormDefinition"/>
/// (BR-074..BR-077). Dispatches type-aware validation per field.
/// </summary>
public static class SubmissionValidator
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^[\+]?[\d\s\-\(\)]{6,20}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Validates that the <paramref name="submissionJson"/> satisfies all
    /// field-level rules of the <paramref name="formDefinition"/>.
    /// Returns a list of validation errors (empty = valid).
    /// </summary>
    public static IReadOnlyList<FieldValidationError> Validate(
        FormDefinition formDefinition,
        string submissionJson)
    {
        var errors = new List<FieldValidationError>();

        Dictionary<string, JsonElement>? values;
        try
        {
            values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(submissionJson);
        }
        catch (JsonException)
        {
            errors.Add(new FieldValidationError("_submission", "Invalid JSON payload."));
            return errors;
        }

        if (values is null)
        {
            errors.Add(new FieldValidationError("_submission", "Submission payload must be a JSON object."));
            return errors;
        }

        foreach (var field in formDefinition.Fields.Where(f => FormFieldTypes.CollectsValue(f.FieldType)))
        {
            values.TryGetValue(field.FieldKey, out var element);
            var hasValue = HasNonEmptyValue(element);

            // BR-074: required validation
            if (field.IsRequired && !hasValue)
            {
                errors.Add(new FieldValidationError(field.FieldKey, $"Field '{field.Label}' is required."));
                continue;
            }

            if (!hasValue) continue;

            // Type-specific validation
            var fieldErrors = ValidateFieldType(field, element);
            errors.AddRange(fieldErrors);
        }

        return errors;
    }

    private static bool HasNonEmptyValue(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Undefined) return false;
        if (element.ValueKind == JsonValueKind.Null) return false;
        if (element.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(element.GetString())) return false;
        if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() == 0) return false;
        return true;
    }

    private static IEnumerable<FieldValidationError> ValidateFieldType(FormField field, JsonElement element)
    {
        switch (field.FieldType)
        {
            case FormFieldType.Email:
                if (element.ValueKind == JsonValueKind.String &&
                    !EmailRegex.IsMatch(element.GetString()!))
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be a valid email address.");
                break;

            case FormFieldType.PhoneNumber:
                if (element.ValueKind == JsonValueKind.String &&
                    !PhoneRegex.IsMatch(element.GetString()!))
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be a valid phone number.");
                break;

            case FormFieldType.Number:
                if (element.ValueKind != JsonValueKind.Number || !element.TryGetInt64(out _))
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be an integer number.");
                break;

            case FormFieldType.Decimal:
                if (element.ValueKind != JsonValueKind.Number)
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be a numeric value.");
                break;

            case FormFieldType.Date:
                if (element.ValueKind != JsonValueKind.String || !DateOnly.TryParse(element.GetString(), out _))
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be a valid date (yyyy-MM-dd).");
                break;

            case FormFieldType.Checkbox:
            case FormFieldType.YesNo:
                if (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False)
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be a boolean value.");
                break;

            case FormFieldType.RadioGroup:
            case FormFieldType.Dropdown:
                if (element.ValueKind == JsonValueKind.String)
                {
                    var val = element.GetString()!;
                    if (field.Options.All(o => !string.Equals(o.Value, val, StringComparison.Ordinal)))
                        yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' has an invalid option value (BR-075).");
                }
                break;

            case FormFieldType.MultiSelect:
                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            var val = item.GetString()!;
                            if (field.Options.All(o => !string.Equals(o.Value, val, StringComparison.Ordinal)))
                                yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' contains an invalid option value '{val}' (BR-075).");
                        }
                    }
                }
                else
                {
                    yield return new FieldValidationError(field.FieldKey, $"Field '{field.Label}' must be an array of values.");
                }
                break;

            case FormFieldType.FileUpload:
                // File upload values are storage references (strings); actual file validation
                // happens at upload time via IFormUploadStorage constraints (BR-076).
                break;

            case FormFieldType.ShortText:
            case FormFieldType.LongText:
                // String types accepted as-is.
                break;
        }
    }
}

/// <summary>Single validation error for a specific field.</summary>
public sealed record FieldValidationError(string FieldKey, string Message);
