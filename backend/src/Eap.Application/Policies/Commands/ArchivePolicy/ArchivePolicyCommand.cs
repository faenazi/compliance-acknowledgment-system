using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.ArchivePolicy;

/// <summary>Archives a policy and any currently-published version (BR-012 preserves history).</summary>
public sealed record ArchivePolicyCommand(Guid PolicyId) : IRequest<PolicyDetailDto>;
