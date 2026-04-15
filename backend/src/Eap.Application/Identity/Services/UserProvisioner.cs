using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Models;
using Eap.Domain.Identity;

namespace Eap.Application.Identity.Services;

/// <summary>
/// Default <see cref="IUserProvisioner"/> implementation.
/// Creates a new user on first login (FR-003) and refreshes AD-derived
/// attributes on every subsequent sync (FR-004, BR-063). Delegates
/// bootstrap role assignment to <see cref="IDefaultRoleAssigner"/>.
/// </summary>
public sealed class UserProvisioner : IUserProvisioner
{
    private readonly IUserRepository _users;
    private readonly IDefaultRoleAssigner _defaultRoles;
    private readonly IIdentityAuditLogger _audit;

    public UserProvisioner(
        IUserRepository users,
        IDefaultRoleAssigner defaultRoles,
        IIdentityAuditLogger audit)
    {
        _users = users;
        _defaultRoles = defaultRoles;
        _audit = audit;
    }

    public async Task<User> ProvisionFromDirectoryAsync(
        LdapUser directoryUser,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directoryUser);

        var existing = await _users
            .FindByUsernameAsync(directoryUser.Username, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            var created = new User(
                username: directoryUser.Username,
                displayName: directoryUser.DisplayName,
                email: directoryUser.Email,
                department: directoryUser.Department,
                jobTitle: directoryUser.JobTitle,
                directoryReference: directoryUser.DirectoryReference);

            await _users.AddAsync(created, cancellationToken).ConfigureAwait(false);
            await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _defaultRoles.ApplyAsync(created, cancellationToken).ConfigureAwait(false);
            await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _audit.UserProvisioned(created.Id, created.Username);
            return created;
        }

        existing.SyncFromDirectory(
            directoryUser.DisplayName,
            directoryUser.Email,
            directoryUser.Department,
            directoryUser.JobTitle,
            directoryUser.DirectoryReference);

        await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.UserSynchronized(existing.Id, existing.Username);
        return existing;
    }
}
