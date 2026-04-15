using Eap.Application.Requirements.Abstractions;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Requirements.Persistence;

internal sealed class RequirementRepository : IRequirementRepository
{
    private readonly EapDbContext _db;

    public RequirementRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UserActionRequirement>> ListForVersionAsync(
        Guid acknowledgmentVersionId,
        UserActionRequirementStatus? status,
        bool currentOnly,
        CancellationToken cancellationToken)
    {
        IQueryable<UserActionRequirement> query = _db.UserActionRequirements
            .AsNoTracking()
            .Where(r => r.AcknowledgmentVersionId == acknowledgmentVersionId);

        if (status is { } s)
        {
            query = query.Where(r => r.Status == s);
        }

        if (currentOnly)
        {
            query = query.Where(r => r.IsCurrent);
        }

        return await query
            .OrderBy(r => r.AssignedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserActionRequirement>> ListLatestForVersionAndCycleAsync(
        Guid acknowledgmentVersionId,
        string cycleReference,
        CancellationToken cancellationToken)
    {
        return await _db.UserActionRequirements
            .Where(r => r.AcknowledgmentVersionId == acknowledgmentVersionId
                        && r.CycleReference == cycleReference)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserActionRequirement>> ListCurrentForVersionAsync(
        Guid acknowledgmentVersionId,
        CancellationToken cancellationToken)
    {
        return await _db.UserActionRequirements
            .Where(r => r.AcknowledgmentVersionId == acknowledgmentVersionId && r.IsCurrent)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddRangeAsync(
        IEnumerable<UserActionRequirement> requirements,
        CancellationToken cancellationToken)
    {
        await _db.UserActionRequirements.AddRangeAsync(requirements, cancellationToken).ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
