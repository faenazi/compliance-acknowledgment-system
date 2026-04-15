using Eap.Application.Requirements.Models;

namespace Eap.Application.Requirements.Abstractions;

/// <summary>
/// Generates <c>UserActionRequirement</c> rows for a published acknowledgment
/// version (BR-040..BR-047, BR-060..BR-063).
///
/// Phase 1 exposes a single explicit generator — no scheduler, no workflow engine.
/// Callers invoke it from the admin UI or from integration tests.
/// </summary>
public interface IRequirementGenerator
{
    /// <summary>
    /// Creates the requirements for the version's current cycle. Idempotent: existing
    /// (user, version, cycle) rows are left alone.
    /// </summary>
    Task<RequirementGenerationSummaryDto> GenerateForVersionAsync(
        Guid acknowledgmentVersionId,
        string? cycleReferenceOverride,
        string? actor,
        CancellationToken cancellationToken);
}
