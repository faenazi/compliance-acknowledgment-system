using Eap.Application.Audit.Abstractions;
using Eap.Domain.Audit;
using Eap.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Audit;

/// <summary>
/// Central DB-backed audit logger (Sprint 8, BR-130 to BR-133).
/// Writes immutable audit records to the AuditLogs table. Also forwards
/// to Serilog for operational log stream continuity.
/// </summary>
internal sealed class PlatformAuditLogger : IPlatformAuditLogger
{
    private readonly EapDbContext _db;
    private readonly ILogger<PlatformAuditLogger> _logger;

    public PlatformAuditLogger(EapDbContext db, ILogger<PlatformAuditLogger> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task LogAsync(
        Guid? actorUserId,
        string? actorUsername,
        string actionType,
        string entityType,
        Guid? entityId,
        Guid? entityVersionId = null,
        string? description = null,
        string? beforeSnapshotJson = null,
        string? afterSnapshotJson = null,
        CancellationToken ct = default)
    {
        var entry = new AuditLog(
            actorUserId,
            actorUsername,
            actionType,
            entityType,
            entityId,
            entityVersionId,
            description,
            beforeSnapshotJson,
            afterSnapshotJson);

        await _db.AuditLogs.AddAsync(entry, ct);
        await _db.SaveChangesAsync(ct);

        // Continue emitting to Serilog for operational log stream continuity.
        _logger.LogInformation(
            "{AuditEvent} {ActionType} {EntityType} {EntityId} {EntityVersionId} {ActorUserId} {Description}",
            "AuditLogRecorded",
            actionType,
            entityType,
            entityId,
            entityVersionId,
            actorUserId,
            description);
    }
}
