using System.DirectoryServices.Protocols;
using Eap.Application.Identity.Models;

namespace Eap.Infrastructure.Identity.Ldap;

/// <summary>
/// Translates a <see cref="SearchResultEntry"/> into a transport-friendly
/// <see cref="LdapUser"/>, honouring the configured attribute mapping.
/// </summary>
internal static class LdapUserAttributeMapper
{
    public static LdapUser Map(SearchResultEntry entry, LdapAttributeMap map)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(map);

        return new LdapUser(
            Username: GetString(entry, map.Username) ?? string.Empty,
            DisplayName: GetString(entry, map.DisplayName) ?? string.Empty,
            Email: GetString(entry, map.Email) ?? string.Empty,
            Department: GetString(entry, map.Department),
            JobTitle: GetString(entry, map.JobTitle),
            DirectoryReference: GetDirectoryReference(entry, map.DirectoryReference),
            Groups: GetStringCollection(entry, map.MemberOf));
    }

    private static string? GetString(SearchResultEntry entry, string attributeName)
    {
        var attr = entry.Attributes[attributeName];
        if (attr is null || attr.Count == 0)
        {
            return null;
        }

        return attr[0] switch
        {
            string s => s,
            byte[] bytes => System.Text.Encoding.UTF8.GetString(bytes),
            _ => attr[0]?.ToString(),
        };
    }

    private static string? GetDirectoryReference(SearchResultEntry entry, string attributeName)
    {
        // objectGUID is returned as raw bytes — preserve a stable hex form.
        var attr = entry.Attributes[attributeName];
        if (attr is null || attr.Count == 0)
        {
            return entry.DistinguishedName;
        }

        return attr[0] switch
        {
            byte[] bytes => new Guid(bytes).ToString(),
            string s => s,
            _ => entry.DistinguishedName,
        };
    }

    private static IReadOnlyCollection<string> GetStringCollection(SearchResultEntry entry, string attributeName)
    {
        var attr = entry.Attributes[attributeName];
        if (attr is null || attr.Count == 0)
        {
            return Array.Empty<string>();
        }

        var values = new List<string>(attr.Count);
        for (var i = 0; i < attr.Count; i++)
        {
            var raw = attr[i];
            var value = raw switch
            {
                string s => s,
                byte[] bytes => System.Text.Encoding.UTF8.GetString(bytes),
                _ => raw?.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(value);
            }
        }

        return values;
    }
}
