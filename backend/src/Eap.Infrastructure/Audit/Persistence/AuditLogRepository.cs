using Eap.Application.Audit.Abstractions;
using Eap.Application.Audit.Models;
using Eap.Domain.Audit;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Audit.Persistence;

internal sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly EapDbContext _db;

    public AuditLogRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLog entry, CancellationToken ct)
    {
        await _db.AuditLogs.AddAsync(entry, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task<(IReadOnlyList<AuditLogDto> Items, int TotalCount)> ListAsync(
        AuditLogFilter filter, CancellationToken ct)
    {
        var query = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.ActionType))
            query = query.Where(a => a.ActionType == filter.ActionType);

        if (!string.IsNullOrWhiteSpace(filter.EntityType))
            query = query.Where(a => a.EntityType == filter.EntityType);

        if (filter.ActorUserId.HasValue)
            query = query.Where(a => a.ActorUserId == filter.ActorUserId.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(a => a.ActionTimestampUtc >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(a => a.ActionTimestampUtc <= filter.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(a =>
                (a.ActorUsername != null && a.ActorUsername.Contains(term))
                || (a.Description != null && a.Description.Contains(term))
                || a.ActionType.Contains(term)
                || a.EntityType.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.ActionTimestampUtc)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                ActorUserId = a.ActorUserId,
                ActorUsername = a.ActorUsername,
                ActionType = a.ActionType,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                EntityVersionId = a.EntityVersionId,
                Description = a.Description,
                ActionTimestampUtc = a.ActionTimestampUtc,
                HasBeforeSnapshot = a.BeforeSnapshotJson != null,
                HasAfterSnapshot = a.AfterSnapshotJson != null,
            })
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
