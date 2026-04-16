namespace Eap.Application.Audit.Abstractions;

/// <summary>
/// Facade for writing audit events to the persistent AuditLog store (Sprint 8).
/// Replaces the per-module Serilog-only audit loggers with a central,
/// DB-backed, immutable audit trail.
/// </summary>
public interface IPlatformAuditLogger
{
    /// <summary>Log a platform audit event to the database.</summary>
    Task LogAsync(
        Guid? actorUserId,
        string? actorUsername,
        string actionType,
        string entityType,
        Guid? entityId,
        Guid? entityVersionId = null,
        string? description = null,
        string? beforeSnapshotJson = null,
        string? afterSnapshotJson = null,
        CancellationToken ct = default);
}
