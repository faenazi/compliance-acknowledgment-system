using Eap.Application.Admin.Models;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminRequirementDetail;

/// <summary>Returns full detail for a single user action requirement (admin view).</summary>
public sealed record GetAdminRequirementDetailQuery(Guid RequirementId) : IRequest<AdminRequirementDetailDto>;
