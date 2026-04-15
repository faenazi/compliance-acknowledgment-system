using Eap.Domain.Identity;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Authenticated user context surfaced to the application layer.
/// <para>
/// Authentication identifies the caller; authorization (roles and scopes) is
/// surfaced here so that handlers can make access decisions without reaching
/// into HTTP or Identity infrastructure. Implementations live in the API layer.
/// </para>
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? Username { get; }

    string? DisplayName { get; }

    string? Email { get; }

    string? Department { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<CurrentUserScope> Scopes { get; }

    bool IsInRole(string roleName);

    bool HasScope(ScopeType scopeType, string? reference = null);
}

/// <summary>
/// Immutable projection of a role-scope pair held by the authenticated user.
/// </summary>
public sealed record CurrentUserScope(ScopeType Type, string Reference, string RoleName);
