using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Services;
using Eap.Domain.Identity;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Identity.Persistence;

/// <summary>
/// Applies configured bootstrap role assignments on first user provisioning.
/// Idempotent — assignments are only added when missing.
/// </summary>
internal sealed class DefaultRoleAssigner : IDefaultRoleAssigner
{
    private readonly EapDbContext _db;
    private readonly IOptions<UserProvisioningOptions> _options;
    private readonly ILogger<DefaultRoleAssigner> _logger;

    public DefaultRoleAssigner(
        EapDbContext db,
        IOptions<UserProvisioningOptions> options,
        ILogger<DefaultRoleAssigner> logger)
    {
        _db = db;
        _options = options;
        _logger = logger;
    }

    public async Task ApplyAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        var options = _options.Value;

        var globalScope = await _db.Scopes
            .FirstOrDefaultAsync(s => s.Type == ScopeType.Global, cancellationToken)
            .ConfigureAwait(false);

        if (globalScope is null)
        {
            _logger.LogWarning("Global scope missing — default role assignment skipped for {Username}.", user.Username);
            return;
        }

        if (options.AssignEndUserRoleOnProvision)
        {
            await EnsureAssignmentAsync(user.Id, SystemRoles.EndUser, globalScope, cancellationToken)
                .ConfigureAwait(false);
        }

        var isBootstrapAdmin = options.SystemAdministrators
            .Any(n => string.Equals(n, user.Username, StringComparison.OrdinalIgnoreCase));

        if (isBootstrapAdmin)
        {
            await EnsureAssignmentAsync(user.Id, SystemRoles.SystemAdministrator, globalScope, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task EnsureAssignmentAsync(
        Guid userId,
        string roleName,
        Scope scope,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken)
            .ConfigureAwait(false);

        if (role is null)
        {
            _logger.LogWarning("Role {Role} is not seeded — assignment skipped for {UserId}.", roleName, userId);
            return;
        }

        var exists = await _db.UserRoleAssignments
            .AnyAsync(a =>
                a.UserId == userId &&
                a.RoleId == role.Id &&
                a.ScopeId == scope.Id &&
                a.IsActive,
                cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            return;
        }

        var assignment = new UserRoleAssignment(
            userId: userId,
            roleId: role.Id,
            scopeId: scope.Id,
            effectiveFromUtc: DateTimeOffset.UtcNow);

        _db.UserRoleAssignments.Add(assignment);
        _logger.LogInformation("Assigned role {Role} to user {UserId}.", roleName, userId);
    }
}
