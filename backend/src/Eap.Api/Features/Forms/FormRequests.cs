using System.ComponentModel.DataAnnotations;
using Eap.Domain.Forms;

namespace Eap.Api.Features.Forms;

/// <summary>API shape for a single field option.</summary>
public sealed class FieldOptionRequest
{
    [Required]
    public string Value { get; set; } = default!;

    [Required]
    public string Label { get; set; } = default!;
}

/// <summary>API shape for a single form field definition.</summary>
public sealed class FormFieldRequest
{
    [Required]
    public string FieldKey { get; set; } = default!;

    [Required]
    public string Label { get; set; } = default!;

    [Required]
    public FormFieldType FieldType { get; set; }

    public bool IsRequired { get; set; }

    public string? SectionKey { get; set; }

    public string? HelpText { get; set; }

    public string? Placeholder { get; set; }

    public string? DisplayText { get; set; }

    public IReadOnlyList<FieldOptionRequest>? Options { get; set; }
}

/// <summary>API contract for configuring a form definition (BR-070..BR-073).</summary>
public sealed class ConfigureFormDefinitionRequest
{
    [Required]
    public IReadOnlyList<FormFieldRequest> Fields { get; set; } = Array.Empty<FormFieldRequest>();
}

/// <summary>API contract for submitting a form-based disclosure (BR-078).</summary>
public sealed class SubmitFormRequest
{
    /// <summary>JSON object with field key → value entries.</summary>
    [Required]
    public string SubmissionJson { get; set; } = default!;
}
