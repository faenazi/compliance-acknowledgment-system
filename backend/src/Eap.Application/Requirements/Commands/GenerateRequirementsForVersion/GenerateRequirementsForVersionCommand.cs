using Eap.Application.Requirements.Models;
using MediatR;

namespace Eap.Application.Requirements.Commands.GenerateRequirementsForVersion;

/// <summary>
/// Explicitly starts a requirement-generation pass for a published version
/// (BR-040..BR-047, BR-060). Phase 1 has no scheduler — admins trigger the pass.
/// </summary>
public sealed record GenerateRequirementsForVersionCommand(
    Guid DefinitionId,
    Guid VersionId,
    string? CycleReference) : IRequest<RequirementGenerationSummaryDto>;
