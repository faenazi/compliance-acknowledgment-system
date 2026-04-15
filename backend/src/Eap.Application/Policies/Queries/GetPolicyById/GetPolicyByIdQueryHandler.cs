using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.GetPolicyById;

public sealed class GetPolicyByIdQueryHandler : IRequestHandler<GetPolicyByIdQuery, PolicyDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IMapper _mapper;

    public GetPolicyByIdQueryHandler(IPolicyRepository policies, IMapper mapper)
    {
        _policies = policies;
        _mapper = mapper;
    }

    public async Task<PolicyDetailDto> Handle(GetPolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        return _mapper.Map<PolicyDetailDto>(policy);
    }
}
