using Eap.Application.Admin.Abstractions;
using Eap.Application.Admin.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminSubmissionDetail;

internal sealed class GetAdminSubmissionDetailQueryHandler
    : IRequestHandler<GetAdminSubmissionDetailQuery, AdminSubmissionDetailDto>
{
    private readonly IAdminRepository _repository;

    public GetAdminSubmissionDetailQueryHandler(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminSubmissionDetailDto> Handle(
        GetAdminSubmissionDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = await _repository.GetSubmissionDetailAsync(request.SubmissionId, cancellationToken);

        return detail ?? throw new NotFoundException(
            nameof(Domain.Forms.UserSubmission), request.SubmissionId);
    }
}
