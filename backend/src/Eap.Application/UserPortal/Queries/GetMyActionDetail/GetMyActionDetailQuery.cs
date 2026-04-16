using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyActionDetail;

/// <summary>Returns full detail for a single requirement assigned to the current user.</summary>
public sealed record GetMyActionDetailQuery(Guid RequirementId) : IRequest<MyActionDetailDto>;
