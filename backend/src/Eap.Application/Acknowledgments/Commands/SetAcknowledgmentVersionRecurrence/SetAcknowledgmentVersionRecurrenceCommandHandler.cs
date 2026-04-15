using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.SetAcknowledgmentVersionRecurrence;

public sealed class SetAcknowledgmentVersionRecurrenceCommandHandler
    : IRequestHandler<SetAcknowledgmentVersionRecurrenceCommand, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public SetAcknowledgmentVersionRecurrenceCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IAcknowledgmentAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentVersionDetailDto> Handle(
        SetAcknowledgmentVersionRecurrenceCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        version.SetRecurrence(
            recurrenceModel: request.RecurrenceModel,
            startDate: request.StartDate,
            dueDate: request.DueDate,
            updatedBy: _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.VersionUpdated(definition.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(version);
    }
}
