namespace Eap.Application.Audience.Abstractions;

/// <summary>
/// Resolves the members of an AD group reference to local user ids.
/// Phase 1 may return an empty list until LDAP group synchronization is wired up.
/// </summary>
public interface IDirectoryGroupResolver
{
    /// <summary>Returns the local user ids that belong to the given AD group reference.</summary>
    Task<IReadOnlyCollection<Guid>> ResolveGroupMembersAsync(
        string groupReference,
        CancellationToken cancellationToken);
}
