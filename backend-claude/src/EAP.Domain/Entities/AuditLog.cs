namespace EAP.Domain.Entities;

public class AuditLog
{
    public Guid AuditLogId { get; set; }
    public Guid? ActorUserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? EntityVersionId { get; set; }
    public DateTime ActionTimestamp { get; set; }
    public string? Description { get; set; }
    public string? BeforeSnapshotJson { get; set; }
    public string? AfterSnapshotJson { get; set; }
}
