using Eap.Domain.Common;

namespace Eap.Domain.Identity;

/// <summary>
/// Local user profile. AD remains the source of truth for identity data
/// (BR-061); this entity caches AD-derived attributes and owns the local
/// records needed for history, role assignments, and audit traceability.
/// </summary>
public sealed class User : Entity
{
    private readonly List<UserRoleAssignment> _roleAssignments = new();

    // EF Core
    private User() { }

    public User(
        string username,
        string displayName,
        string email,
        string? department,
        string? jobTitle,
        string? directoryReference)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        Username = username.Trim().ToLowerInvariant();
        DisplayName = (displayName ?? username).Trim();
        Email = (email ?? string.Empty).Trim();
        Department = department?.Trim();
        JobTitle = jobTitle?.Trim();
        DirectoryReference = directoryReference?.Trim();
        IsActive = true;
        LastSyncedAtUtc = DateTimeOffset.UtcNow;
    }

    public string Username { get; private set; } = default!;

    public string DisplayName { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    public string? Department { get; private set; }

    public string? JobTitle { get; private set; }

    /// <summary>
    /// Opaque reference to the AD/LDAP source record (e.g. objectGUID, distinguished name).
    /// </summary>
    public string? DirectoryReference { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset? LastLoginAtUtc { get; private set; }

    public DateTimeOffset LastSyncedAtUtc { get; private set; }

    public IReadOnlyCollection<UserRoleAssignment> RoleAssignments => _roleAssignments.AsReadOnly();

    /// <summary>
    /// Applies AD-sourced attributes to the local profile. Identity attributes
    /// are always overwritten from AD (BR-061) — local overrides are not allowed.
    /// </summary>
    public void SyncFromDirectory(
        string displayName,
        string email,
        string? department,
        string? jobTitle,
        string? directoryReference)
    {
        DisplayName = (displayName ?? Username).Trim();
        Email = (email ?? string.Empty).Trim();
        Department = department?.Trim();
        JobTitle = jobTitle?.Trim();
        if (!string.IsNullOrWhiteSpace(directoryReference))
        {
            DirectoryReference = directoryReference.Trim();
        }

        LastSyncedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = LastSyncedAtUtc;
    }

    public void RecordSuccessfulLogin(DateTimeOffset whenUtc)
    {
        LastLoginAtUtc = whenUtc;
        UpdatedAtUtc = whenUtc;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    internal void AttachAssignment(UserRoleAssignment assignment)
    {
        _roleAssignments.Add(assignment);
    }
}
