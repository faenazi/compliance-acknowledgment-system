namespace Eap.Application.Requirements.Abstractions;

public interface IRequirementAuditLogger
{
    void RequirementsGenerated(
        Guid acknowledgmentVersionId,
        string cycleReference,
        int createdCount,
        int skippedCount,
        int cancelledCount,
        string? actor);
}
