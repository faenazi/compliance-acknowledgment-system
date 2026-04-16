namespace Eap.Domain.Audit;

/// <summary>
/// Immutable audit record capturing a critical business event
/// (BR-130 to BR-133, CDM §9.3). Once created, audit logs must
/// not be editable through standard business interfaces.
/// </summary>
public sealed class AuditLog
{
    // EF Core
    private AuditLog() { }

    public AuditLog(
        Guid? actorUserId,
        string? actorUsername,
        string actionType,
        string entityType,
        Guid? entityId,
        Guid? entityVersionId,
        string? description,
        string? beforeSnapshotJson,
        string? afterSnapshotJson)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            throw new ArgumentException("Action type is required.", nameof(actionType));
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("Entity type is required.", nameof(entityType));
        }

        Id = Guid.NewGuid();
        ActorUserId = actorUserId;
        ActorUsername = actorUsername?.Trim();
        ActionType = actionType.Trim();
        EntityType = entityType.Trim();
        EntityId = entityId;
        EntityVersionId = entityVersionId;
        Description = description?.Trim();
        BeforeSnapshotJson = beforeSnapshotJson;
        AfterSnapshotJson = afterSnapshotJson;
        ActionTimestampUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    /// <summary>The user who performed the action. Null for system-initiated actions.</summary>
    public Guid? ActorUserId { get; private set; }

    /// <summary>Displayable actor name captured at event time (denormalized for readability).</summary>
    public string? ActorUsername { get; private set; }

    /// <summary>Category of action performed — e.g. PolicyCreated, UserSubmission, LoginSucceeded.</summary>
    public string ActionType { get; private set; } = default!;

    /// <summary>Type of entity affected — e.g. Policy, AcknowledgmentVersion, UserSubmission.</summary>
    public string EntityType { get; private set; } = default!;

    /// <summary>Primary key of the affected entity.</summary>
    public Guid? EntityId { get; private set; }

    /// <summary>Version key of the affected entity when applicable.</summary>
    public Guid? EntityVersionId { get; private set; }

    /// <summary>Human-readable description of the event.</summary>
    public string? Description { get; private set; }

    /// <summary>Optional JSON snapshot of the entity state before the action.</summary>
    public string? BeforeSnapshotJson { get; private set; }

    /// <summary>Optional JSON snapshot of the entity state after the action.</summary>
    public string? AfterSnapshotJson { get; private set; }

    /// <summary>UTC timestamp when the event occurred (immutable).</summary>
    public DateTimeOffset ActionTimestampUtc { get; private set; }
}
