using System.DirectoryServices.Protocols;
using System.Net;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Identity.Ldap;

/// <summary>
/// LDAP directory lookup without credential validation.
/// Shared by <see cref="LdapAuthenticationService"/> (to resolve the user DN
/// before binding) and by profile synchronization flows.
/// </summary>
internal sealed class LdapUserDirectory : IUserDirectory
{
    private readonly LdapConnectionFactory _connectionFactory;
    private readonly IOptions<LdapOptions> _options;
    private readonly ILogger<LdapUserDirectory> _logger;

    public LdapUserDirectory(
        LdapConnectionFactory connectionFactory,
        IOptions<LdapOptions> options,
        ILogger<LdapUserDirectory> logger)
    {
        _connectionFactory = connectionFactory;
        _options = options;
        _logger = logger;
    }

    public Task<LdapUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        var credential = BuildServiceCredential(opts);

        cancellationToken.ThrowIfCancellationRequested();

        using var connection = _connectionFactory.Create(credential);
        try
        {
            if (credential is not null)
            {
                connection.Bind();
            }

            var entry = SearchUser(connection, opts, username);
            if (entry is null)
            {
                return Task.FromResult<LdapUser?>(null);
            }

            var result = LdapUserAttributeMapper.Map(entry, opts.Attributes);
            return Task.FromResult<LdapUser?>(result);
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP directory lookup failed for user {Username}", username);
            throw;
        }
    }

    internal SearchResultEntry? SearchUser(LdapConnection connection, LdapOptions opts, string username)
    {
        var filter = opts.UserSearchFilter.Replace("{username}", EscapeFilterValue(username));
        var attributes = new[]
        {
            opts.Attributes.Username,
            opts.Attributes.DisplayName,
            opts.Attributes.Email,
            opts.Attributes.Department,
            opts.Attributes.JobTitle,
            opts.Attributes.MemberOf,
            opts.Attributes.DirectoryReference,
        };

        var request = new SearchRequest(
            opts.BaseDn,
            filter,
            SearchScope.Subtree,
            attributes);

        var response = (SearchResponse)connection.SendRequest(request);
        return response.Entries.Count == 0 ? null : response.Entries[0];
    }

    private static NetworkCredential? BuildServiceCredential(LdapOptions opts)
    {
        if (string.IsNullOrWhiteSpace(opts.BindDn))
        {
            return null;
        }

        return new NetworkCredential(opts.BindDn, opts.BindPassword ?? string.Empty);
    }

    // Minimal RFC 4515 escaping — prevents injection via the filter placeholder.
    private static string EscapeFilterValue(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length);
        foreach (var ch in value)
        {
            switch (ch)
            {
                case '\\': builder.Append("\\5c"); break;
                case '*': builder.Append("\\2a"); break;
                case '(': builder.Append("\\28"); break;
                case ')': builder.Append("\\29"); break;
                case '\0': builder.Append("\\00"); break;
                default: builder.Append(ch); break;
            }
        }

        return builder.ToString();
    }
}
