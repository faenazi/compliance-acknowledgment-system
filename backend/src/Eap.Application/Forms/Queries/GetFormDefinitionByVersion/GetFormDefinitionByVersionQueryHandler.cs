using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.GetFormDefinitionByVersion;

public sealed class GetFormDefinitionByVersionQueryHandler
    : IRequestHandler<GetFormDefinitionByVersionQuery, FormDefinitionDto?>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public GetFormDefinitionByVersionQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<FormDefinitionDto?> Handle(
        GetFormDefinitionByVersionQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        return version.FormDefinition is null
            ? null
            : _mapper.Map<FormDefinitionDto>(version.FormDefinition);
    }
}
