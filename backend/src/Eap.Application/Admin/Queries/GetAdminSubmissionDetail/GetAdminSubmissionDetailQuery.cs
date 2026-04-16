using Eap.Application.Admin.Models;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminSubmissionDetail;

/// <summary>Returns full detail for any user's submission (admin view, not scoped to current user).</summary>
public sealed record GetAdminSubmissionDetailQuery(Guid SubmissionId) : IRequest<AdminSubmissionDetailDto>;
