using Eap.Domain.Acknowledgment;

namespace Eap.Application.Acknowledgments.Abstractions;

/// <summary>
/// Persistence abstraction for the <see cref="AcknowledgmentDefinition"/> aggregate.
/// Aggregate loads eagerly include versions so domain invariants (single-published)
/// can be enforced in memory.
/// </summary>
public interface IAcknowledgmentRepository
{
    Task<AcknowledgmentDefinition?> FindByIdAsync(Guid definitionId, CancellationToken cancellationToken);

    /// <summary>Loads the parent definition (with its versions and audiences) for a given version id.</summary>
    Task<AcknowledgmentDefinition?> FindDefinitionByVersionIdAsync(Guid versionId, CancellationToken cancellationToken);

    /// <summary>Highest version number ever used for the given definition; 0 when none exist yet.</summary>
    Task<int> GetMaxVersionNumberAsync(Guid definitionId, CancellationToken cancellationToken);

    Task<(IReadOnlyList<AcknowledgmentDefinition> Items, int TotalCount)> ListAsync(
        AcknowledgmentListFilter filter,
        CancellationToken cancellationToken);

    Task AddAsync(AcknowledgmentDefinition definition, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>Filter / paging contract for listing acknowledgment definitions.</summary>
public sealed record AcknowledgmentListFilter(
    int Page,
    int PageSize,
    string? Search,
    AcknowledgmentStatus? Status,
    string? OwnerDepartment,
    ActionType? ActionType);
