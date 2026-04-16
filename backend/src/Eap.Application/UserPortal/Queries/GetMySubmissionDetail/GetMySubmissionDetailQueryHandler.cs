using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMySubmissionDetail;

public sealed class GetMySubmissionDetailQueryHandler
    : IRequestHandler<GetMySubmissionDetailQuery, MySubmissionDetailDto>
{
    private readonly IUserPortalRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMySubmissionDetailQueryHandler(
        IUserPortalRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<MySubmissionDetailDto> Handle(
        GetMySubmissionDetailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        var detail = await _repository.GetSubmissionDetailAsync(userId, request.SubmissionId, cancellationToken)
            ?? throw new NotFoundException("UserSubmission", request.SubmissionId);

        return detail;
    }
}
