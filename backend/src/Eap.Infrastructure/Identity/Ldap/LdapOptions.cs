using System.ComponentModel.DataAnnotations;

namespace Eap.Infrastructure.Identity.Ldap;

/// <summary>
/// Strongly-typed LDAP / Active Directory configuration.
/// Bound from the <c>Ldap</c> section in configuration. Sensitive values
/// (<see cref="BindPassword"/>) must be supplied via environment variables
/// or a secret store, never via checked-in appsettings files.
/// </summary>
public sealed class LdapOptions
{
    public const string SectionName = "Ldap";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 389;

    /// <summary>Enable LDAPS / StartTLS. Strongly recommended in production.</summary>
    public bool UseSsl { get; set; }

    /// <summary>Windows domain (NETBIOS/DNS) used for DOMAIN\user binds when applicable.</summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Base distinguished name for directory searches (e.g. <c>DC=tef,DC=local</c>).
    /// </summary>
    [Required]
    public string BaseDn { get; set; } = string.Empty;

    /// <summary>
    /// Service account DN used to perform directory lookups prior to user bind.
    /// Optional — leave empty when direct user binding is sufficient.
    /// </summary>
    public string? BindDn { get; set; }

    /// <summary>
    /// Service account password. Must be supplied via environment variables
    /// or a secret store. Never commit real values.
    /// </summary>
    public string? BindPassword { get; set; }

    /// <summary>
    /// LDAP search filter for resolving a user by username.
    /// The placeholder <c>{username}</c> is replaced at runtime.
    /// </summary>
    public string UserSearchFilter { get; set; } =
        "(&(objectClass=user)(sAMAccountName={username}))";

    /// <summary>Operation timeout in seconds.</summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 15;

    /// <summary>Attribute mapping between AD attributes and platform fields.</summary>
    public LdapAttributeMap Attributes { get; set; } = new();
}

/// <summary>
/// Configurable mapping between Active Directory attribute names and the
/// fields consumed by the platform. Defaults follow standard AD schema.
/// </summary>
public sealed class LdapAttributeMap
{
    public string Username { get; set; } = "sAMAccountName";
    public string DisplayName { get; set; } = "displayName";
    public string Email { get; set; } = "mail";
    public string Department { get; set; } = "department";
    public string JobTitle { get; set; } = "title";
    public string MemberOf { get; set; } = "memberOf";
    public string DirectoryReference { get; set; } = "objectGUID";
}
