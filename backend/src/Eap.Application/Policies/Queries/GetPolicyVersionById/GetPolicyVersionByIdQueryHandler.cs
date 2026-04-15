using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.GetPolicyVersionById;

public sealed class GetPolicyVersionByIdQueryHandler
    : IRequestHandler<GetPolicyVersionByIdQuery, PolicyVersionDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IMapper _mapper;

    public GetPolicyVersionByIdQueryHandler(IPolicyRepository policies, IMapper mapper)
    {
        _policies = policies;
        _mapper = mapper;
    }

    public async Task<PolicyVersionDetailDto> Handle(
        GetPolicyVersionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        var version = policy.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("PolicyVersion", request.VersionId);

        return _mapper.Map<PolicyVersionDetailDto>(version);
    }
}
