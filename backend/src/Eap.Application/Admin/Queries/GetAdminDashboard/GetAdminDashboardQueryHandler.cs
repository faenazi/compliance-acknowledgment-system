using Eap.Application.Admin.Abstractions;
using Eap.Application.Admin.Models;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminDashboard;

internal sealed class GetAdminDashboardQueryHandler
    : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IAdminRepository _repository;

    public GetAdminDashboardQueryHandler(IAdminRepository repository)
    {
        _repository = repository;
    }

    public Task<AdminDashboardDto> Handle(
        GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetDashboardAsync(request.RecentLimit, cancellationToken);
    }
}
