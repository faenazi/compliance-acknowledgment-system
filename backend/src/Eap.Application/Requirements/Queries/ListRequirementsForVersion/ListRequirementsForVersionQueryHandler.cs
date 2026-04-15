using AutoMapper;
using Eap.Application.Requirements.Abstractions;
using Eap.Application.Requirements.Models;
using MediatR;

namespace Eap.Application.Requirements.Queries.ListRequirementsForVersion;

public sealed class ListRequirementsForVersionQueryHandler
    : IRequestHandler<ListRequirementsForVersionQuery, IReadOnlyList<UserActionRequirementDto>>
{
    private readonly IRequirementRepository _requirements;
    private readonly IMapper _mapper;

    public ListRequirementsForVersionQueryHandler(
        IRequirementRepository requirements,
        IMapper mapper)
    {
        _requirements = requirements;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserActionRequirementDto>> Handle(
        ListRequirementsForVersionQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _requirements
            .ListForVersionAsync(request.VersionId, request.Status, request.CurrentOnly, cancellationToken)
            .ConfigureAwait(false);

        return items
            .Select(_mapper.Map<UserActionRequirementDto>)
            .ToList();
    }
}
