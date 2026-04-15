using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.CreatePolicyVersion;

/// <summary>Creates a new draft version for the given policy.</summary>
public sealed record CreatePolicyVersionCommand(
    Guid PolicyId,
    string? VersionLabel,
    DateOnly? EffectiveDate,
    string? Summary) : IRequest<PolicyVersionDetailDto>;
