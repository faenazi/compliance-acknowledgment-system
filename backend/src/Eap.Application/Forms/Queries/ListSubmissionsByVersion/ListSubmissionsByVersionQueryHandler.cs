using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.ListSubmissionsByVersion;

public sealed class ListSubmissionsByVersionQueryHandler
    : IRequestHandler<ListSubmissionsByVersionQuery, ListSubmissionsByVersionResult>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IUserSubmissionRepository _submissions;
    private readonly IMapper _mapper;

    public ListSubmissionsByVersionQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IUserSubmissionRepository submissions,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _submissions = submissions;
        _mapper = mapper;
    }

    public async Task<ListSubmissionsByVersionResult> Handle(
        ListSubmissionsByVersionQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        _ = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        var (items, totalCount) = await _submissions
            .ListByVersionAsync(request.VersionId, request.Page, request.PageSize, cancellationToken)
            .ConfigureAwait(false);

        var dtos = _mapper.Map<IReadOnlyList<UserSubmissionSummaryDto>>(items);
        return new ListSubmissionsByVersionResult(dtos, totalCount);
    }
}
