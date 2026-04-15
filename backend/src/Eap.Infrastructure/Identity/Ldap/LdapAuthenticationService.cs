using System.DirectoryServices.Protocols;
using System.Net;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Identity.Ldap;

/// <summary>
/// LDAP / Active Directory credential-bind authentication.
/// <para>
/// The service resolves the target user through a directory search (using the
/// service account if configured, otherwise an anonymous lookup) and then
/// performs a second bind using the user's own DN and password — the only
/// way to prove credential validity without relying on DOMAIN\user shortcuts
/// that may not be available in non-Windows hosts.
/// </para>
/// </summary>
internal sealed class LdapAuthenticationService : ILdapAuthenticator
{
    private readonly LdapConnectionFactory _connectionFactory;
    private readonly LdapUserDirectory _directory;
    private readonly IOptions<LdapOptions> _options;
    private readonly ILogger<LdapAuthenticationService> _logger;

    public LdapAuthenticationService(
        LdapConnectionFactory connectionFactory,
        LdapUserDirectory directory,
        IOptions<LdapOptions> options,
        ILogger<LdapAuthenticationService> logger)
    {
        _connectionFactory = connectionFactory;
        _directory = directory;
        _options = options;
        _logger = logger;
    }

    public Task<LdapUser?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            return Task.FromResult<LdapUser?>(null);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var opts = _options.Value;
        SearchResultEntry? entry;

        try
        {
            using var lookupConnection = _connectionFactory.Create(BuildServiceCredential(opts));
            if (!string.IsNullOrWhiteSpace(opts.BindDn))
            {
                lookupConnection.Bind();
            }

            entry = _directory.SearchUser(lookupConnection, opts, username);
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP lookup failed for user {Username}", username);
            throw;
        }

        if (entry is null)
        {
            return Task.FromResult<LdapUser?>(null);
        }

        // Verify credentials with a dedicated bind as the resolved user DN.
        try
        {
            using var userConnection = _connectionFactory.Create(
                new NetworkCredential(entry.DistinguishedName, password));
            userConnection.Bind();
        }
        catch (LdapException ex) when (ex.ErrorCode == 49)
        {
            // 49 = invalidCredentials.
            return Task.FromResult<LdapUser?>(null);
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP bind failed for user {Username}", username);
            throw;
        }

        return Task.FromResult<LdapUser?>(LdapUserAttributeMapper.Map(entry, opts.Attributes));
    }

    private static NetworkCredential? BuildServiceCredential(LdapOptions opts)
    {
        if (string.IsNullOrWhiteSpace(opts.BindDn))
        {
            return null;
        }

        return new NetworkCredential(opts.BindDn, opts.BindPassword ?? string.Empty);
    }
}
