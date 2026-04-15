using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.PublishPolicyVersion;

/// <summary>Transitions a draft version to Published (BR-010, BR-011).</summary>
public sealed record PublishPolicyVersionCommand(Guid PolicyId, Guid VersionId) : IRequest<PolicyVersionDetailDto>;
