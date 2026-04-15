using Eap.Application.Policies.Abstractions;
using Eap.Domain.Policy;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Policies.Persistence;

internal sealed class PolicyRepository : IPolicyRepository
{
    private readonly EapDbContext _db;

    public PolicyRepository(EapDbContext db)
    {
        _db = db;
    }

    public Task<Policy?> FindByIdAsync(Guid policyId, CancellationToken cancellationToken) =>
        _db.Policies
            .Include(p => p.Versions)
                .ThenInclude(v => v.Document)
            .FirstOrDefaultAsync(p => p.Id == policyId, cancellationToken);

    public Task<Policy?> FindByCodeAsync(string policyCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(policyCode))
        {
            return Task.FromResult<Policy?>(null);
        }

        var normalized = policyCode.Trim();
        return _db.Policies
            .Include(p => p.Versions)
                .ThenInclude(v => v.Document)
            .FirstOrDefaultAsync(p => p.PolicyCode == normalized, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        string policyCode,
        Guid? excludingPolicyId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(policyCode))
        {
            return Task.FromResult(false);
        }

        var normalized = policyCode.Trim();
        var query = _db.Policies.AsQueryable().Where(p => p.PolicyCode == normalized);
        if (excludingPolicyId is { } excludeId)
        {
            query = query.Where(p => p.Id != excludeId);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetMaxVersionNumberAsync(Guid policyId, CancellationToken cancellationToken)
    {
        // Returning int instead of int? — an empty set surfaces as 0 which is exactly
        // what the caller wants to add 1 to for the first draft.
        var any = await _db.PolicyVersions
            .AnyAsync(v => v.PolicyId == policyId, cancellationToken)
            .ConfigureAwait(false);

        if (!any)
        {
            return 0;
        }

        return await _db.PolicyVersions
            .Where(v => v.PolicyId == policyId)
            .MaxAsync(v => v.VersionNumber, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<(IReadOnlyList<Policy> Items, int TotalCount)> ListAsync(
        PolicyListFilter filter,
        CancellationToken cancellationToken)
    {
        IQueryable<Policy> query = _db.Policies
            .Include(p => p.Versions)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.PolicyCode, $"%{search}%")
                || EF.Functions.Like(p.Title, $"%{search}%"));
        }

        if (filter.Status is { } status)
        {
            query = query.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.OwnerDepartment))
        {
            var owner = filter.OwnerDepartment.Trim();
            query = query.Where(p => p.OwnerDepartment == owner);
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            var category = filter.Category.Trim();
            query = query.Where(p => p.Category == category);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderBy(p => p.Title)
            .ThenBy(p => p.PolicyCode)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task AddAsync(Policy policy, CancellationToken cancellationToken)
    {
        await _db.Policies.AddAsync(policy, cancellationToken).ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
