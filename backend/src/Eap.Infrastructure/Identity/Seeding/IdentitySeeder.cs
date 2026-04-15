using Eap.Domain.Identity;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Identity.Seeding;

/// <summary>
/// Seeds the system roles listed in <see cref="SystemRoles"/> and a default
/// Global scope. Idempotent — safe to invoke on every application start.
/// </summary>
public sealed class IdentitySeeder
{
    private readonly EapDbContext _db;
    private readonly IOptions<IdentitySeedOptions> _options;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        EapDbContext db,
        IOptions<IdentitySeedOptions> options,
        ILogger<IdentitySeeder> logger)
    {
        _db = db;
        _options = options;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (!_options.Value.Enabled)
        {
            _logger.LogInformation("Identity seed skipped (Identity:Seed:Enabled = false).");
            return;
        }

        if (!await _db.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false))
        {
            _logger.LogWarning("Identity seed skipped: database is not reachable.");
            return;
        }

        await SeedRolesAsync(cancellationToken).ConfigureAwait(false);
        await SeedGlobalScopeAsync(cancellationToken).ConfigureAwait(false);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var existing = await _db.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingSet = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in SystemRoles.All)
        {
            if (existingSet.Contains(name))
            {
                continue;
            }

            _db.Roles.Add(new Role(name, description: null, isSystemRole: true));
            _logger.LogInformation("Seeded system role {Role}", name);
        }
    }

    private async Task SeedGlobalScopeAsync(CancellationToken cancellationToken)
    {
        var exists = await _db.Scopes
            .AnyAsync(s => s.Type == ScopeType.Global, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            return;
        }

        _db.Scopes.Add(new Scope(ScopeType.Global, reference: null, description: "Platform-wide access."));
        _logger.LogInformation("Seeded Global scope.");
    }
}
