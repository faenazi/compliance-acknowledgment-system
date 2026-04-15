using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentDefinition;

public sealed class UpdateAcknowledgmentDefinitionCommandHandler
    : IRequestHandler<UpdateAcknowledgmentDefinitionCommand, AcknowledgmentDefinitionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UpdateAcknowledgmentDefinitionCommandHandler(
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

    public async Task<AcknowledgmentDefinitionDetailDto> Handle(
        UpdateAcknowledgmentDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments.FindByIdAsync(request.DefinitionId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        definition.UpdateMetadata(
            title: request.Title,
            ownerDepartment: request.OwnerDepartment,
            defaultActionType: request.DefaultActionType,
            description: request.Description,
            updatedBy: _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.DefinitionMetadataUpdated(definition.Id, definition.Title, _currentUser.Username);

        return _mapper.Map<AcknowledgmentDefinitionDetailDto>(definition);
    }
}
