namespace Eap.Application.Audit.Models;

/// <summary>Audit log entry for the explorer table.</summary>
public sealed class AuditLogDto
{
    public Guid Id { get; init; }
    public Guid? ActorUserId { get; init; }
    public string? ActorUsername { get; init; }
    public string ActionType { get; init; } = default!;
    public string EntityType { get; init; } = default!;
    public Guid? EntityId { get; init; }
    public Guid? EntityVersionId { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset ActionTimestampUtc { get; init; }
    public bool HasBeforeSnapshot { get; init; }
    public bool HasAfterSnapshot { get; init; }
}

/// <summary>Full audit log detail including optional snapshots.</summary>
public sealed class AuditLogDetailDto
{
    public Guid Id { get; init; }
    public Guid? ActorUserId { get; init; }
    public string? ActorUsername { get; init; }
    public string ActionType { get; init; } = default!;
    public string EntityType { get; init; } = default!;
    public Guid? EntityId { get; init; }
    public Guid? EntityVersionId { get; init; }
    public string? Description { get; init; }
    public string? BeforeSnapshotJson { get; init; }
    public string? AfterSnapshotJson { get; init; }
    public DateTimeOffset ActionTimestampUtc { get; init; }
}

/// <summary>Filter parameters for audit log queries.</summary>
public sealed record AuditLogFilter(
    int Page,
    int PageSize,
    string? ActionType = null,
    string? EntityType = null,
    Guid? ActorUserId = null,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null,
    string? Search = null);
