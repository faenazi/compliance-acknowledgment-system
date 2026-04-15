namespace Eap.Domain.Audience;

/// <summary>
/// Rule types supported in Phase 1 (BR-050 / FR-050).
/// Kept as a closed enum — no generic attribute engine — so the evaluator
/// remains deterministic and reviewable.
/// </summary>
public enum AudienceRuleType
{
    /// <summary>Match every user in the directory (BR-051).</summary>
    AllUsers = 0,

    /// <summary>Match users whose AD-derived department equals the rule value (BR-052).</summary>
    Department = 1,

    /// <summary>Match users who belong to the AD group referenced by the rule value (BR-053).</summary>
    AdGroup = 2,

    /// <summary>Explicit single-user targeting (used primarily on the exclusion side, BR-054).</summary>
    User = 3,
}
