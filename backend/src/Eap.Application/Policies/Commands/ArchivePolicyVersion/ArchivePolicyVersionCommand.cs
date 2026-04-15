using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.ArchivePolicyVersion;

/// <summary>Archives a draft or superseded version (BR-012).</summary>
public sealed record ArchivePolicyVersionCommand(Guid PolicyId, Guid VersionId) : IRequest<PolicyVersionDetailDto>;
