namespace Eap.Domain.Audience;

/// <summary>
/// High-level audience authoring mode (docs/05-data/conceptual-data-model.md §7.1).
/// Used by the admin UI to drive the rules editor; the effective membership is
/// always computed from <see cref="AudienceRule"/> rows regardless of this flag.
/// </summary>
public enum AudienceType
{
    /// <summary>One rule matching every user (BR-051).</summary>
    AllUsers = 0,

    /// <summary>One or more rules limiting to specific departments (BR-052).</summary>
    Departments = 1,

    /// <summary>One or more rules limiting to specific AD groups (BR-053).</summary>
    AdGroups = 2,

    /// <summary>Mixed — departments, groups, and/or explicit user targeting.</summary>
    Mixed = 3,
}
