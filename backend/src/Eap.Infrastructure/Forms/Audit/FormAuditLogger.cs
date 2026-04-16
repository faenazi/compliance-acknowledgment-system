using Eap.Application.Forms.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Forms.Audit;

/// <summary>
/// Sprint 5 audit sink for form-definition and submission events. Writes
/// structured Serilog events tagged with <c>AuditEvent</c>.
/// </summary>
internal sealed class FormAuditLogger : IFormAuditLogger
{
    private readonly ILogger<FormAuditLogger> _logger;

    public FormAuditLogger(ILogger<FormAuditLogger> logger)
    {
        _logger = logger;
    }

    public void FormDefinitionConfigured(
        Guid definitionId, Guid versionId, Guid formDefinitionId, int fieldCount, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {FormDefinitionId} {FieldCount} {Actor}",
            "FormDefinitionConfigured", definitionId, versionId, formDefinitionId, fieldCount, actor ?? "-");

    public void FormSubmissionCreated(
        Guid submissionId, Guid userId, Guid versionId, Guid formDefinitionId, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {SubmissionId} {UserId} {VersionId} {FormDefinitionId} {Actor}",
            "FormSubmissionCreated", submissionId, userId, versionId, formDefinitionId, actor ?? "-");
}
