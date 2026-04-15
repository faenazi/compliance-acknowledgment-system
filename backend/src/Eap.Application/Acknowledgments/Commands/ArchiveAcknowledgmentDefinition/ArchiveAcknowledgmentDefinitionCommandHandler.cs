using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentDefinition;

public sealed class ArchiveAcknowledgmentDefinitionCommandHandler
    : IRequestHandler<ArchiveAcknowledgmentDefinitionCommand, AcknowledgmentDefinitionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public ArchiveAcknowledgmentDefinitionCommandHandler(
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

    public async Task<AcknowledgmentDefinitionDetailDto> Handle(
        ArchiveAcknowledgmentDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments.FindByIdAsync(request.DefinitionId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        definition.Archive(_currentUser.Username, _clock.GetUtcNow());

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.DefinitionArchived(definition.Id, definition.Title, _currentUser.Username);

        return _mapper.Map<AcknowledgmentDefinitionDetailDto>(definition);
    }
}
