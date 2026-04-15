using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Identity.Ldap;

/// <summary>
/// Creates configured <see cref="LdapConnection"/> instances. Centralised so
/// that connection shape (SSL, protocol version, timeout) is consistent and
/// configurable across authentication and directory-lookup paths.
/// </summary>
internal sealed class LdapConnectionFactory
{
    private readonly IOptions<LdapOptions> _options;

    public LdapConnectionFactory(IOptions<LdapOptions> options)
    {
        _options = options;
    }

    public LdapConnection Create(NetworkCredential? credential = null)
    {
        var opts = _options.Value;
        var identifier = new LdapDirectoryIdentifier(opts.Host, opts.Port, fullyQualifiedDnsHostName: false, connectionless: false);

        var connection = new LdapConnection(identifier)
        {
            AuthType = AuthType.Basic,
            Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds),
        };

        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = opts.UseSsl;

        if (credential is not null)
        {
            connection.Credential = credential;
        }

        return connection;
    }
}
