using Eap.Application.Requirements.Models;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.Requirements.Queries.ListRequirementsForVersion;

/// <summary>
/// Lists requirements attached to a given acknowledgment version. Filters are
/// intentionally minimal for Phase 1 — admins use this to spot-check a generation pass.
/// </summary>
public sealed record ListRequirementsForVersionQuery(
    Guid DefinitionId,
    Guid VersionId,
    UserActionRequirementStatus? Status,
    bool CurrentOnly) : IRequest<IReadOnlyList<UserActionRequirementDto>>;
