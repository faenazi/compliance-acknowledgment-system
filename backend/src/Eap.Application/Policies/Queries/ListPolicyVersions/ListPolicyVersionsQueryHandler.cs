using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.ListPolicyVersions;

public sealed class ListPolicyVersionsQueryHandler
    : IRequestHandler<ListPolicyVersionsQuery, IReadOnlyList<PolicyVersionSummaryDto>>
{
    private readonly IPolicyRepository _policies;
    private readonly IMapper _mapper;

    public ListPolicyVersionsQueryHandler(IPolicyRepository policies, IMapper mapper)
    {
        _policies = policies;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PolicyVersionSummaryDto>> Handle(
        ListPolicyVersionsQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        return _mapper.Map<IReadOnlyList<PolicyVersionSummaryDto>>(
            policy.Versions.OrderByDescending(v => v.VersionNumber).ToList());
    }
}
