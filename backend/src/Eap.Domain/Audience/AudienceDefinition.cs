using Eap.Domain.Common;

namespace Eap.Domain.Audience;

/// <summary>
/// Audience targeting rules owned by an <c>AcknowledgmentVersion</c>
/// (docs/05-data/conceptual-data-model.md §7.1, BR-050..BR-055).
///
/// A definition holds an ordered list of <see cref="AudienceRule"/> entries.
/// Inclusion rules are unioned to produce the candidate set; exclusion rules
/// are then subtracted (BR-055). At least one inclusion rule is required
/// before the owning acknowledgment version can be published (BR-032).
/// </summary>
public sealed class AudienceDefinition : AuditableEntity
{
    private readonly List<AudienceRule> _rules = new();

    // EF Core
    private AudienceDefinition() { }

    internal AudienceDefinition(Guid acknowledgmentVersionId, string? createdBy)
    {
        if (acknowledgmentVersionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Acknowledgment version id is required.",
                nameof(acknowledgmentVersionId));
        }

        AcknowledgmentVersionId = acknowledgmentVersionId;
        AudienceType = AudienceType.AllUsers;
        CreatedBy = createdBy?.Trim();
    }

    public Guid AcknowledgmentVersionId { get; private set; }

    /// <summary>High-level authoring mode. Recomputed whenever rules change.</summary>
    public AudienceType AudienceType { get; private set; }

    public IReadOnlyCollection<AudienceRule> Rules => _rules.AsReadOnly();

    /// <summary>Inclusion rules only (BR-050).</summary>
    public IEnumerable<AudienceRule> InclusionRules =>
        _rules.Where(r => !r.IsExclusion).OrderBy(r => r.SortOrder);

    /// <summary>Exclusion rules only (BR-054).</summary>
    public IEnumerable<AudienceRule> ExclusionRules =>
        _rules.Where(r => r.IsExclusion).OrderBy(r => r.SortOrder);

    /// <summary>True when at least one inclusion rule exists — required for publish (BR-032).</summary>
    public bool HasAnyInclusionRule => _rules.Any(r => !r.IsExclusion);

    /// <summary>
    /// Replaces the audience with a single "all users" inclusion rule (BR-051).
    /// Existing rules are cleared.
    /// </summary>
    public void SetAllUsers(string? updatedBy)
    {
        _rules.Clear();
        _rules.Add(new AudienceRule(
            audienceDefinitionId: Id,
            ruleType: AudienceRuleType.AllUsers,
            ruleValue: null,
            isExclusion: false,
            sortOrder: 0,
            createdBy: updatedBy));

        AudienceType = AudienceType.AllUsers;
        Touch(updatedBy);
    }

    /// <summary>
    /// Replaces all inclusion rules with the supplied department/AD-group/user rules,
    /// preserving existing exclusion rules. Empty <paramref name="inclusionRules"/>
    /// leaves the audience with no inclusion — publish will then be blocked.
    /// </summary>
    public void ReplaceInclusionRules(IEnumerable<AudienceRuleInput> inclusionRules, string? updatedBy)
    {
        ArgumentNullException.ThrowIfNull(inclusionRules);

        _rules.RemoveAll(r => !r.IsExclusion);
        var index = 0;
        foreach (var input in inclusionRules)
        {
            if (input.IsExclusion)
            {
                throw new ArgumentException(
                    "Use ReplaceExclusionRules to configure exclusion rules.",
                    nameof(inclusionRules));
            }

            _rules.Add(new AudienceRule(
                audienceDefinitionId: Id,
                ruleType: input.RuleType,
                ruleValue: input.RuleValue,
                isExclusion: false,
                sortOrder: index++,
                createdBy: updatedBy));
        }

        AudienceType = DeriveAudienceType();
        Touch(updatedBy);
    }

    /// <summary>
    /// Replaces all exclusion rules with the supplied values (BR-054 / BR-055).
    /// Inclusion rules are left untouched.
    /// </summary>
    public void ReplaceExclusionRules(IEnumerable<AudienceRuleInput> exclusionRules, string? updatedBy)
    {
        ArgumentNullException.ThrowIfNull(exclusionRules);

        _rules.RemoveAll(r => r.IsExclusion);
        var index = 0;
        foreach (var input in exclusionRules)
        {
            if (!input.IsExclusion)
            {
                throw new ArgumentException(
                    "Use ReplaceInclusionRules to configure inclusion rules.",
                    nameof(exclusionRules));
            }

            if (input.RuleType == AudienceRuleType.AllUsers)
            {
                // Excluding "everyone" would make the audience trivially empty — reject it up front.
                throw new InvalidOperationException(
                    "An 'All Users' rule cannot be used as an exclusion (BR-055).");
            }

            _rules.Add(new AudienceRule(
                audienceDefinitionId: Id,
                ruleType: input.RuleType,
                ruleValue: input.RuleValue,
                isExclusion: true,
                sortOrder: index++,
                createdBy: updatedBy));
        }

        Touch(updatedBy);
    }

    private AudienceType DeriveAudienceType()
    {
        var inclusions = _rules.Where(r => !r.IsExclusion).Select(r => r.RuleType).Distinct().ToList();
        if (inclusions.Count == 0)
        {
            return AudienceType.AllUsers;
        }

        if (inclusions.Count == 1)
        {
            return inclusions[0] switch
            {
                AudienceRuleType.AllUsers => AudienceType.AllUsers,
                AudienceRuleType.Department => AudienceType.Departments,
                AudienceRuleType.AdGroup => AudienceType.AdGroups,
                _ => AudienceType.Mixed,
            };
        }

        return AudienceType.Mixed;
    }

    private void Touch(string? updatedBy)
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }
}

/// <summary>
/// Input shape used to batch-replace rules on an <see cref="AudienceDefinition"/>.
/// Kept inside the domain namespace so both the domain and application layers can
/// refer to it without a mapping step.
/// </summary>
public readonly record struct AudienceRuleInput(
    AudienceRuleType RuleType,
    string? RuleValue,
    bool IsExclusion);
