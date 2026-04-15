using Eap.Domain.Common;

namespace Eap.Domain.Identity;

/// <summary>
/// Organizational scope attached to a role assignment.
/// <para>
/// A Scope is a typed pointer: <see cref="Type"/> classifies the scope, and
/// <see cref="Reference"/> carries the contextual value (e.g. department key
/// for <see cref="ScopeType.Department"/>). Global scopes carry no reference.
/// </para>
/// </summary>
public sealed class Scope : Entity
{
    // EF Core
    private Scope() { }

    public Scope(ScopeType type, string? reference, string? description)
    {
        Type = type;
        Reference = NormalizeReference(type, reference);
        Description = description?.Trim();
    }

    public ScopeType Type { get; private set; }

    /// <summary>
    /// Contextual reference for the scope.
    /// Empty string for <see cref="ScopeType.Global"/> and <see cref="ScopeType.OwnedContent"/>.
    /// </summary>
    public string Reference { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    private static string NormalizeReference(ScopeType type, string? reference) => type switch
    {
        ScopeType.Global or ScopeType.OwnedContent => string.Empty,
        ScopeType.Department when string.IsNullOrWhiteSpace(reference)
            => throw new ArgumentException("Department scope requires a reference value.", nameof(reference)),
        _ => reference!.Trim(),
    };
}
