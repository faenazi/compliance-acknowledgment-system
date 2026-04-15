namespace Eap.Domain.Requirements;

/// <summary>
/// Lifecycle states for a <see cref="UserActionRequirement"/>
/// (docs/03-functional-requirements/lifecycle-models.md §5).
/// </summary>
public enum UserActionRequirementStatus
{
    /// <summary>Generated and waiting for the user to act (BR-060).</summary>
    Pending = 0,

    /// <summary>User acknowledged / submitted — no further action expected (BR-061).</summary>
    Completed = 1,

    /// <summary>Due date passed without completion (BR-062). Phase 1 does not block access.</summary>
    Overdue = 2,

    /// <summary>Superseded or explicitly cancelled (BR-063) — usually because the user left
    /// the audience or the owning version was archived.</summary>
    Cancelled = 3,
}
