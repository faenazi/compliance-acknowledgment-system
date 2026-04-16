using Eap.Domain.Forms;

namespace Eap.Application.Forms.Models;

/// <summary>DTO for a single field option (select/radio/multi-select).</summary>
public sealed class FieldOptionDto
{
    public string Value { get; init; } = default!;
    public string Label { get; init; } = default!;
}

/// <summary>DTO for a single form field.</summary>
public sealed class FormFieldDto
{
    public Guid Id { get; init; }
    public string FieldKey { get; init; } = default!;
    public string Label { get; init; } = default!;
    public FormFieldType FieldType { get; init; }
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
    public string? SectionKey { get; init; }
    public string? HelpText { get; init; }
    public string? Placeholder { get; init; }
    public string? DisplayText { get; init; }
    public IReadOnlyList<FieldOptionDto> Options { get; init; } = Array.Empty<FieldOptionDto>();
}

/// <summary>DTO for a complete form definition.</summary>
public sealed class FormDefinitionDto
{
    public Guid Id { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public int SchemaVersion { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<FormFieldDto> Fields { get; init; } = Array.Empty<FormFieldDto>();
    public DateTimeOffset CreatedAtUtc { get; init; }
    public string? CreatedBy { get; init; }
    public DateTimeOffset? UpdatedAtUtc { get; init; }
    public string? UpdatedBy { get; init; }
}

/// <summary>Input shape for a single field when configuring a form definition.</summary>
public sealed class FormFieldInputDto
{
    public string FieldKey { get; set; } = default!;
    public string Label { get; set; } = default!;
    public FormFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public string? SectionKey { get; set; }
    public string? HelpText { get; set; }
    public string? Placeholder { get; set; }
    public string? DisplayText { get; set; }
    public IReadOnlyList<FieldOptionDto>? Options { get; set; }
}
