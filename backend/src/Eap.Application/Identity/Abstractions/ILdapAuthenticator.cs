using Eap.Application.Identity.Models;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Performs LDAP / Active Directory authentication (credential bind)
/// and returns the directory attributes required for local provisioning.
/// Implementations live in the Infrastructure layer.
/// </summary>
public interface ILdapAuthenticator
{
    /// <summary>
    /// Attempts to authenticate the supplied credentials against the directory.
    /// </summary>
    /// <returns>
    /// The resolved directory user on success; <see langword="null"/> when the
    /// credentials are invalid or the user cannot be located. Infrastructure
    /// failures (connectivity, TLS, misconfiguration) should be thrown.
    /// </returns>
    Task<LdapUser?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken);
}
