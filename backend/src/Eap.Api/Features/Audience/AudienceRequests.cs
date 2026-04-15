using System.ComponentModel.DataAnnotations;
using Eap.Domain.Audience;

namespace Eap.Api.Features.Audience;

/// <summary>API shape for a single audience rule (inclusion or exclusion).</summary>
public sealed class AudienceRuleRequest
{
    [Required]
    public AudienceRuleType RuleType { get; set; }

    public string? RuleValue { get; set; }
}

/// <summary>API contract for replacing the inclusion rules on a draft audience (BR-050..BR-053).</summary>
public sealed class ConfigureAudienceInclusionRequest
{
    [Required]
    public IReadOnlyList<AudienceRuleRequest> Rules { get; set; } = Array.Empty<AudienceRuleRequest>();
}

/// <summary>API contract for replacing the exclusion rules on a draft audience (BR-054 / BR-055).</summary>
public sealed class ConfigureAudienceExclusionsRequest
{
    [Required]
    public IReadOnlyList<AudienceRuleRequest> Rules { get; set; } = Array.Empty<AudienceRuleRequest>();
}
