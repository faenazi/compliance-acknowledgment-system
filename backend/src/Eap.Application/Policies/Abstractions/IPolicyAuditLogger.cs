namespace Eap.Application.Policies.Abstractions;

/// <summary>
/// Narrow audit surface for policy-management events. In the MVP this writes
/// structured Serilog events tagged with <c>AuditEvent</c>; a dedicated
/// immutable AuditLog persistence store is introduced in a later sprint
/// (BR-130, BR-132) without requiring callers of this interface to change.
/// </summary>
public interface IPolicyAuditLogger
{
    void PolicyCreated(Guid policyId, string policyCode, string? actor);

    void PolicyMetadataUpdated(Guid policyId, string policyCode, string? actor);

    void PolicyArchived(Guid policyId, string policyCode, string? actor);

    void PolicyVersionCreated(Guid policyId, Guid versionId, int versionNumber, string? actor);

    void PolicyVersionUpdated(Guid policyId, Guid versionId, int versionNumber, string? actor);

    void PolicyVersionPublished(Guid policyId, Guid versionId, int versionNumber, string? actor);

    void PolicyVersionArchived(Guid policyId, Guid versionId, int versionNumber, string? actor);

    void PolicyDocumentUploaded(
        Guid policyId,
        Guid versionId,
        Guid documentId,
        string fileName,
        long fileSize,
        string? actor);

    void PolicyDocumentDownloaded(
        Guid policyId,
        Guid versionId,
        Guid documentId,
        string fileName,
        string? actor);
}
