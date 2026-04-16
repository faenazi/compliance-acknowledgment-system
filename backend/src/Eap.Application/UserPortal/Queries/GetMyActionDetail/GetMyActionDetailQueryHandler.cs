using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyActionDetail;

public sealed class GetMyActionDetailQueryHandler
    : IRequestHandler<GetMyActionDetailQuery, MyActionDetailDto>
{
    private readonly IUserPortalRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMyActionDetailQueryHandler(
        IUserPortalRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<MyActionDetailDto> Handle(
        GetMyActionDetailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        var detail = await _repository.GetActionDetailAsync(userId, request.RequirementId, cancellationToken)
            ?? throw new NotFoundException("UserActionRequirement", request.RequirementId);

        return detail;
    }
}
