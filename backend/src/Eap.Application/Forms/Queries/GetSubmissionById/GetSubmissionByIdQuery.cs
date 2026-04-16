using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.GetSubmissionById;

public sealed record GetSubmissionByIdQuery(Guid SubmissionId) : IRequest<UserSubmissionDetailDto>;
