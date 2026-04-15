using Eap.Domain.Common;

namespace Eap.Domain.Audience;

/// <summary>
/// A single inclusion or exclusion rule under an <see cref="AudienceDefinition"/>
/// (docs/05-data/conceptual-data-model.md §7.2, BR-050..BR-055).
///
/// Rules are evaluated as: (union of inclusion rules) minus (union of exclusion rules).
/// Explicit exclusions always override inclusions (BR-055).
/// </summary>
public sealed class AudienceRule : AuditableEntity
{
    // EF Core
    private AudienceRule() { }

    internal AudienceRule(
        Guid audienceDefinitionId,
        AudienceRuleType ruleType,
        string? ruleValue,
        bool isExclusion,
        int sortOrder,
        string? createdBy)
    {
        if (audienceDefinitionId == Guid.Empty)
        {
            throw new ArgumentException("Audience definition id is required.", nameof(audienceDefinitionId));
        }

        ValidateRuleValue(ruleType, ruleValue);

        AudienceDefinitionId = audienceDefinitionId;
        RuleType = ruleType;
        RuleValue = Normalize(ruleValue);
        IsExclusion = isExclusion;
        SortOrder = sortOrder;
        CreatedBy = createdBy?.Trim();
    }

    public Guid AudienceDefinitionId { get; private set; }

    public AudienceRuleType RuleType { get; private set; }

    /// <summary>Department name, AD group identifier, or user id — depending on
    /// <see cref="RuleType"/>. Null for <see cref="AudienceRuleType.AllUsers"/>.</summary>
    public string? RuleValue { get; private set; }

    /// <summary>When true this rule subtracts from the audience (BR-054 / BR-055).</summary>
    public bool IsExclusion { get; private set; }

    public int SortOrder { get; private set; }

    internal void UpdateSortOrder(int sortOrder, string? updatedBy)
    {
        SortOrder = sortOrder;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }

    private static string? Normalize(string? ruleValue) =>
        string.IsNullOrWhiteSpace(ruleValue) ? null : ruleValue.Trim();

    private static void ValidateRuleValue(AudienceRuleType ruleType, string? ruleValue)
    {
        switch (ruleType)
        {
            case AudienceRuleType.AllUsers:
                // AllUsers has no payload — values are ignored.
                return;

            case AudienceRuleType.Department:
            case AudienceRuleType.AdGroup:
            case AudienceRuleType.User:
                if (string.IsNullOrWhiteSpace(ruleValue))
                {
                    throw new ArgumentException(
                        $"A rule value is required for rule type {ruleType}.",
                        nameof(ruleValue));
                }
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, "Unknown audience rule type.");
        }
    }
}
