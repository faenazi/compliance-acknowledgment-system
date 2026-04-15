using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.UpdatePolicyVersionDraft;

/// <summary>Updates editable fields on a draft version (BR-003). Ignored for non-drafts.</summary>
public sealed record UpdatePolicyVersionDraftCommand(
    Guid PolicyId,
    Guid VersionId,
    string? VersionLabel,
    DateOnly? EffectiveDate,
    string? Summary) : IRequest<PolicyVersionDetailDto>;
