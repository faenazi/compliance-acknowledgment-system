using Eap.Domain.Requirements;

namespace Eap.Application.Requirements.Abstractions;

/// <summary>
/// Persistence abstraction for <see cref="UserActionRequirement"/> rows.
/// Generation passes rely on bulk lookups (latest open cycle for a version) and
/// a simple upsert surface — no aggregate is required.
/// </summary>
public interface IRequirementRepository
{
    Task<IReadOnlyList<UserActionRequirement>> ListForVersionAsync(
        Guid acknowledgmentVersionId,
        UserActionRequirementStatus? status,
        bool currentOnly,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<UserActionRequirement>> ListLatestForVersionAndCycleAsync(
        Guid acknowledgmentVersionId,
        string cycleReference,
        CancellationToken cancellationToken);

    /// <summary>Used by generation passes to flag the prior cycle's rows as not-current.</summary>
    Task<IReadOnlyList<UserActionRequirement>> ListCurrentForVersionAsync(
        Guid acknowledgmentVersionId,
        CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<UserActionRequirement> requirements, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
