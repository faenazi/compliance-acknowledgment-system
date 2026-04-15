using Eap.Application.Identity.Models;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Read-only directory lookup. Used by scheduled profile synchronization
/// and by administrative tooling that needs to resolve a user without
/// performing a credential bind.
/// </summary>
public interface IUserDirectory
{
    Task<LdapUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken);
}
