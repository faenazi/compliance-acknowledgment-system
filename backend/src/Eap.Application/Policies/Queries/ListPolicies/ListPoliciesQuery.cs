using Eap.Application.Common.Models;
using Eap.Application.Policies.Models;
using Eap.Domain.Policy;
using MediatR;

namespace Eap.Application.Policies.Queries.ListPolicies;

/// <summary>Paged list of policies with optional status/owner/category filters.</summary>
public sealed record ListPoliciesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    PolicyStatus? Status = null,
    string? OwnerDepartment = null,
    string? Category = null) : IRequest<PagedResult<PolicySummaryDto>>;
