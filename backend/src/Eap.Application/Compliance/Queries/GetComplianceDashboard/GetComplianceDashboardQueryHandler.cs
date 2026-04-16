using Eap.Application.Compliance.Abstractions;
using Eap.Application.Compliance.Models;
using MediatR;

namespace Eap.Application.Compliance.Queries.GetComplianceDashboard;

internal sealed class GetComplianceDashboardQueryHandler
    : IRequestHandler<GetComplianceDashboardQuery, ComplianceDashboardDto>
{
    private readonly IComplianceRepository _repository;

    public GetComplianceDashboardQueryHandler(IComplianceRepository repository)
    {
        _repository = repository;
    }

    public Task<ComplianceDashboardDto> Handle(
        GetComplianceDashboardQuery request, CancellationToken cancellationToken)
    {
        var filter = new ComplianceDashboardFilter(
            Department: request.Department,
            AcknowledgmentDefinitionId: request.AcknowledgmentDefinitionId,
            PolicyId: request.PolicyId,
            TopNonCompliantLimit: request.TopNonCompliantLimit);

        return _repository.GetDashboardAsync(filter, cancellationToken);
    }
}
