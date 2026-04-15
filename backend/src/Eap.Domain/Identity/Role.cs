using Eap.Domain.Common;

namespace Eap.Domain.Identity;

/// <summary>
/// Platform authorization role. Roles are local application data and are
/// independent of LDAP groups (BR-141, BR-142).
/// </summary>
public sealed class Role : Entity
{
    // EF Core
    private Role() { }

    public Role(string name, string? description, bool isSystemRole)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim();
        IsSystemRole = isSystemRole;
    }

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    /// <summary>
    /// Indicates a role provisioned by the platform seed and protected from deletion.
    /// </summary>
    public bool IsSystemRole { get; private set; }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
