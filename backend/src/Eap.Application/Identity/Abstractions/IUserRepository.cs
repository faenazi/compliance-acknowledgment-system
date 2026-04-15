using Eap.Domain.Identity;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Persistence abstraction for <see cref="User"/> and its role assignments.
/// The concrete EF Core implementation lives in the Infrastructure layer.
/// </summary>
public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the effective (role-name, scope-type, scope-reference) tuples for
    /// the given user, filtered by active assignments that are currently in effect.
    /// </summary>
    Task<IReadOnlyList<UserRoleAssignmentView>> GetActiveAssignmentsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Flat projection of an assignment used for authorization context.
/// Kept as a simple record to avoid exposing EF entities through the abstraction.
/// </summary>
public sealed record UserRoleAssignmentView(
    string RoleName,
    ScopeType ScopeType,
    string ScopeReference);
