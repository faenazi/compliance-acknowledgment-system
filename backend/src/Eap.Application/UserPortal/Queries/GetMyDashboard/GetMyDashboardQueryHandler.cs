using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyDashboard;

public sealed class GetMyDashboardQueryHandler
    : IRequestHandler<GetMyDashboardQuery, MyDashboardDto>
{
    private readonly IUserPortalRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMyDashboardQueryHandler(
        IUserPortalRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<MyDashboardDto> Handle(
        GetMyDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        return await _repository.GetDashboardAsync(
            userId, request.PendingLimit, request.RecentLimit, cancellationToken);
    }
}
