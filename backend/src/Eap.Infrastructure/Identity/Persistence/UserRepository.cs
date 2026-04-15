using Eap.Application.Identity.Abstractions;
using Eap.Domain.Identity;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Identity.Persistence;

internal sealed class UserRepository : IUserRepository
{
    private readonly EapDbContext _db;

    public UserRepository(EapDbContext db)
    {
        _db = db;
    }

    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult<User?>(null);
        }

        var normalized = username.Trim().ToLowerInvariant();
        return _db.Users
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Username == normalized, cancellationToken);
    }

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _db.Users
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserRoleAssignmentView>> GetActiveAssignmentsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var asOf = DateTimeOffset.UtcNow;

        var rows = await (
            from assignment in _db.UserRoleAssignments
            join role in _db.Roles on assignment.RoleId equals role.Id
            join scope in _db.Scopes on assignment.ScopeId equals scope.Id
            where assignment.UserId == userId
                  && assignment.IsActive
                  && assignment.EffectiveFromUtc <= asOf
                  && (assignment.EffectiveToUtc == null || assignment.EffectiveToUtc > asOf)
            select new UserRoleAssignmentView(role.Name, scope.Type, scope.Reference))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
