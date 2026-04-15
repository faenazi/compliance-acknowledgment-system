namespace Eap.Application.Acknowledgments.Abstractions;

/// <summary>
/// Narrow audit surface for acknowledgment-management events. Writes structured
/// Serilog events tagged with <c>AuditEvent</c>; a dedicated immutable AuditLog
/// store (BR-130/132) replaces this sink in a later sprint without callers changing.
/// </summary>
public interface IAcknowledgmentAuditLogger
{
    void DefinitionCreated(Guid definitionId, string title, string? actor);

    void DefinitionMetadataUpdated(Guid definitionId, string title, string? actor);

    void DefinitionArchived(Guid definitionId, string title, string? actor);

    void VersionCreated(Guid definitionId, Guid versionId, int versionNumber, Guid policyVersionId, string? actor);

    void VersionUpdated(Guid definitionId, Guid versionId, int versionNumber, string? actor);

    void VersionPublished(Guid definitionId, Guid versionId, int versionNumber, string? actor);

    void VersionArchived(Guid definitionId, Guid versionId, int versionNumber, string? actor);
}
