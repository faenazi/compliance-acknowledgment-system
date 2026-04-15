using Eap.Application.Acknowledgments.Abstractions;
using Eap.Domain.Acknowledgment;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Acknowledgments.Persistence;

internal sealed class AcknowledgmentRepository : IAcknowledgmentRepository
{
    private readonly EapDbContext _db;

    public AcknowledgmentRepository(EapDbContext db)
    {
        _db = db;
    }

    public Task<AcknowledgmentDefinition?> FindByIdAsync(
        Guid definitionId,
        CancellationToken cancellationToken) =>
        _db.AcknowledgmentDefinitions
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == definitionId, cancellationToken);

    public async Task<int> GetMaxVersionNumberAsync(
        Guid definitionId,
        CancellationToken cancellationToken)
    {
        var any = await _db.AcknowledgmentVersions
            .AnyAsync(v => v.AcknowledgmentDefinitionId == definitionId, cancellationToken)
            .ConfigureAwait(false);

        if (!any)
        {
            return 0;
        }

        return await _db.AcknowledgmentVersions
            .Where(v => v.AcknowledgmentDefinitionId == definitionId)
            .MaxAsync(v => v.VersionNumber, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<(IReadOnlyList<AcknowledgmentDefinition> Items, int TotalCount)> ListAsync(
        AcknowledgmentListFilter filter,
        CancellationToken cancellationToken)
    {
        IQueryable<AcknowledgmentDefinition> query = _db.AcknowledgmentDefinitions
            .Include(d => d.Versions)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(d => EF.Functions.Like(d.Title, $"%{search}%"));
        }

        if (filter.Status is { } status)
        {
            query = query.Where(d => d.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.OwnerDepartment))
        {
            var owner = filter.OwnerDepartment.Trim();
            query = query.Where(d => d.OwnerDepartment == owner);
        }

        if (filter.ActionType is { } actionType)
        {
            query = query.Where(d => d.DefaultActionType == actionType);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderBy(d => d.Title)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task AddAsync(AcknowledgmentDefinition definition, CancellationToken cancellationToken)
    {
        await _db.AcknowledgmentDefinitions.AddAsync(definition, cancellationToken).ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
