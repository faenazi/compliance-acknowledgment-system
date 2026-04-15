using Eap.Domain.Audience;

namespace Eap.Application.Audience.Models;

/// <summary>DTO for a single audience rule (inclusion or exclusion).</summary>
public sealed class AudienceRuleDto
{
    public Guid Id { get; init; }

    public AudienceRuleType RuleType { get; init; }

    public string? RuleValue { get; init; }

    public bool IsExclusion { get; init; }

    public int SortOrder { get; init; }
}

/// <summary>DTO for the audience configuration attached to an acknowledgment version.</summary>
public sealed class AudienceDefinitionDto
{
    public Guid Id { get; init; }

    public Guid AcknowledgmentVersionId { get; init; }

    public AudienceType AudienceType { get; init; }

    public IReadOnlyList<AudienceRuleDto> InclusionRules { get; init; } = Array.Empty<AudienceRuleDto>();

    public IReadOnlyList<AudienceRuleDto> ExclusionRules { get; init; } = Array.Empty<AudienceRuleDto>();

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }

    public string? UpdatedBy { get; init; }
}

/// <summary>Input for a single audience rule on configure commands.</summary>
public sealed record AudienceRuleInputDto(
    AudienceRuleType RuleType,
    string? RuleValue);

/// <summary>Projection returned by PreviewAudience: matched users + counts.</summary>
public sealed class AudiencePreviewDto
{
    public int EstimatedUserCount { get; init; }

    public int InclusionMatchedCount { get; init; }

    public int ExclusionMatchedCount { get; init; }

    public IReadOnlyList<AudiencePreviewUserDto> SampleUsers { get; init; } = Array.Empty<AudiencePreviewUserDto>();
}

public sealed class AudiencePreviewUserDto
{
    public Guid UserId { get; init; }

    public string Username { get; init; } = default!;

    public string DisplayName { get; init; } = default!;

    public string? Department { get; init; }
}
