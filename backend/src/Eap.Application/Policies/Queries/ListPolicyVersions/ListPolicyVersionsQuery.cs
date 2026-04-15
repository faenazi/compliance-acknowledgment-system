using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.ListPolicyVersions;

public sealed record ListPolicyVersionsQuery(Guid PolicyId)
    : IRequest<IReadOnlyList<PolicyVersionSummaryDto>>;
