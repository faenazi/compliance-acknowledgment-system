using Eap.Application.Common.Models;
using Eap.Application.Compliance.Models;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.Compliance.Queries.ListNonCompliantUsers;

public sealed record ListNonCompliantUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Department = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    UserActionRequirementStatus? Status = null,
    string? Search = null) : IRequest<PagedResult<NonCompliantUserDetailDto>>;
