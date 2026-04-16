using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMySubmissionDetail;

/// <summary>Returns full detail for a single past submission owned by the current user.</summary>
public sealed record GetMySubmissionDetailQuery(Guid SubmissionId) : IRequest<MySubmissionDetailDto>;
