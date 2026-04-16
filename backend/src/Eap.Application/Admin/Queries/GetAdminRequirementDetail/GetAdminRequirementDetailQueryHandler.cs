using Eap.Application.Admin.Abstractions;
using Eap.Application.Admin.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminRequirementDetail;

internal sealed class GetAdminRequirementDetailQueryHandler
    : IRequestHandler<GetAdminRequirementDetailQuery, AdminRequirementDetailDto>
{
    private readonly IAdminRepository _repository;

    public GetAdminRequirementDetailQueryHandler(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminRequirementDetailDto> Handle(
        GetAdminRequirementDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = await _repository.GetRequirementDetailAsync(request.RequirementId, cancellationToken);

        return detail ?? throw new NotFoundException(
            nameof(Domain.Requirements.UserActionRequirement), request.RequirementId);
    }
}
