namespace Eap.Domain.Identity;

/// <summary>
/// Organizational scope classifications supported by the platform.
/// Documented in conceptual-data-model.md §4.3 and business-rules BR-144.
/// </summary>
public enum ScopeType
{
    /// <summary>Access applies across the whole platform.</summary>
    Global = 0,

    /// <summary>Access is limited to one organizational department.</summary>
    Department = 1,

    /// <summary>Access is limited to content owned/authored by the user.</summary>
    OwnedContent = 2,
}
