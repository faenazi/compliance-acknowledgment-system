using Eap.Domain.Common;

namespace Eap.Domain.Identity;

/// <summary>
/// Associates a <see cref="User"/> with a <see cref="Role"/> and a <see cref="Scope"/>.
/// A user may hold multiple assignments simultaneously (BR-140, BR-144).
/// </summary>
public sealed class UserRoleAssignment : Entity
{
    // EF Core
    private UserRoleAssignment() { }

    public UserRoleAssignment(
        Guid userId,
        Guid roleId,
        Guid scopeId,
        DateTimeOffset effectiveFromUtc,
        DateTimeOffset? effectiveToUtc = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (roleId == Guid.Empty) throw new ArgumentException("RoleId is required.", nameof(roleId));
        if (scopeId == Guid.Empty) throw new ArgumentException("ScopeId is required.", nameof(scopeId));

        UserId = userId;
        RoleId = roleId;
        ScopeId = scopeId;
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        IsActive = true;
    }

    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public Guid ScopeId { get; private set; }

    public DateTimeOffset EffectiveFromUtc { get; private set; }

    public DateTimeOffset? EffectiveToUtc { get; private set; }

    public bool IsActive { get; private set; }

    public void Deactivate()
    {
        IsActive = false;
        EffectiveToUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = EffectiveToUtc;
    }

    public bool IsCurrentlyEffective(DateTimeOffset asOfUtc) =>
        IsActive
        && EffectiveFromUtc <= asOfUtc
        && (!EffectiveToUtc.HasValue || EffectiveToUtc.Value > asOfUtc);
}
