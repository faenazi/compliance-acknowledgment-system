using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.GetPolicyById;

public sealed record GetPolicyByIdQuery(Guid PolicyId) : IRequest<PolicyDetailDto>;
