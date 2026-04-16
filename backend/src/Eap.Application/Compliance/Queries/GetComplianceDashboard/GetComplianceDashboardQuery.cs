using Eap.Application.Compliance.Models;
using MediatR;

namespace Eap.Application.Compliance.Queries.GetComplianceDashboard;

public sealed record GetComplianceDashboardQuery(
    string? Department = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    int TopNonCompliantLimit = 10) : IRequest<ComplianceDashboardDto>;
