namespace Eap.Application.Forms.Abstractions;

/// <summary>
/// Audit surface for form-definition and submission operations.
/// Writes Serilog events tagged with <c>AuditEvent</c>.
/// </summary>
public interface IFormAuditLogger
{
    void FormDefinitionConfigured(Guid definitionId, Guid versionId, Guid formDefinitionId, int fieldCount, string? actor);

    void FormSubmissionCreated(Guid submissionId, Guid userId, Guid versionId, Guid formDefinitionId, string? actor);
}
