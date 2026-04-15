using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentVersion;

public sealed class ArchiveAcknowledgmentVersionCommandHandler
    : IRequestHandler<ArchiveAcknowledgmentVersionCommand, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public ArchiveAcknowledgmentVersionCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IAcknowledgmentAuditLogger audit,
        ICurrentUser currentUser,
        TimeProvider clock,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _audit = audit;
        _currentUser = currentUser;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentVersionDetailDto> Handle(
        ArchiveAcknowledgmentVersionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        definition.ArchiveVersion(request.VersionId, _currentUser.Username, _clock.GetUtcNow());

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var version = definition.Versions.Single(v => v.Id == request.VersionId);
        _audit.VersionArchived(definition.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(version);
    }
}
