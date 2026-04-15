using Eap.Application.Identity.Models;
using Eap.Domain.Identity;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Creates or refreshes a local <see cref="User"/> profile from a directory snapshot.
/// Separated from authentication so that both login-time and scheduled sync
/// flows can share the same logic.
/// </summary>
public interface IUserProvisioner
{
    Task<User> ProvisionFromDirectoryAsync(LdapUser directoryUser, CancellationToken cancellationToken);
}
