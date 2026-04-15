namespace Eap.Domain.Common;

/// <summary>
/// Extends <see cref="Entity"/> with audit attribution fields.
/// Used by domain entities that need to record who created or modified them.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public string? CreatedBy { get; protected set; }

    public string? UpdatedBy { get; protected set; }
}
