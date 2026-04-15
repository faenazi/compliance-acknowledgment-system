using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.GetPolicyVersionById;

public sealed record GetPolicyVersionByIdQuery(Guid PolicyId, Guid VersionId)
    : IRequest<PolicyVersionDetailDto>;
