using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.GetSubmissionById;

public sealed class GetSubmissionByIdQueryHandler
    : IRequestHandler<GetSubmissionByIdQuery, UserSubmissionDetailDto>
{
    private readonly IUserSubmissionRepository _submissions;
    private readonly IMapper _mapper;

    public GetSubmissionByIdQueryHandler(
        IUserSubmissionRepository submissions,
        IMapper mapper)
    {
        _submissions = submissions;
        _mapper = mapper;
    }

    public async Task<UserSubmissionDetailDto> Handle(
        GetSubmissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var submission = await _submissions
            .FindByIdAsync(request.SubmissionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("UserSubmission", request.SubmissionId);

        return _mapper.Map<UserSubmissionDetailDto>(submission);
    }
}
