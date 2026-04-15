using Eap.Domain.Policy;

namespace Eap.Application.Policies.Abstractions;

/// <summary>
/// Persistence abstraction for the <see cref="Policy"/> aggregate. The concrete
/// EF Core implementation lives in the Infrastructure layer. Aggregate loads
/// eagerly include <see cref="PolicyVersion"/>s and their attached documents so
/// domain invariants (BR-010, BR-011) can be enforced in memory.
/// </summary>
public interface IPolicyRepository
{
    Task<Policy?> FindByIdAsync(Guid policyId, CancellationToken cancellationToken);

    Task<Policy?> FindByCodeAsync(string policyCode, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(string policyCode, Guid? excludingPolicyId, CancellationToken cancellationToken);

    /// <summary>Highest version number ever used for a given policy; 0 when none exist yet.</summary>
    Task<int> GetMaxVersionNumberAsync(Guid policyId, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Policy> Items, int TotalCount)> ListAsync(
        PolicyListFilter filter,
        CancellationToken cancellationToken);

    Task AddAsync(Policy policy, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>Filter / paging contract for listing policies.</summary>
public sealed record PolicyListFilter(
    int Page,
    int PageSize,
    string? Search,
    PolicyStatus? Status,
    string? OwnerDepartment,
    string? Category);
