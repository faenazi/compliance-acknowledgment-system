namespace Eap.Domain.Common;

/// <summary>
/// Base type for persistent domain entities.
/// Kept intentionally minimal for Sprint 0; concrete entities are introduced per module in later sprints.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; protected set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAtUtc { get; protected set; }
}
