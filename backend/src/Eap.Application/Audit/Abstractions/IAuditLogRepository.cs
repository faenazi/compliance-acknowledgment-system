using Eap.Application.Audit.Models;

namespace Eap.Application.Audit.Abstractions;

/// <summary>
/// Repository for immutable audit log records (BR-130 to BR-133, Sprint 8).
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>Persist a new audit log entry. Audit records are immutable once written.</summary>
    Task AddAsync(Domain.Audit.AuditLog entry, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);

    /// <summary>Paginated, filtered list of audit log entries.</summary>
    Task<(IReadOnlyList<AuditLogDto> Items, int TotalCount)> ListAsync(
        AuditLogFilter filter, CancellationToken ct);
}
