using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Identity.Abstractions;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentDefinition;

public sealed class CreateAcknowledgmentDefinitionCommandHandler
    : IRequestHandler<CreateAcknowledgmentDefinitionCommand, AcknowledgmentDefinitionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public CreateAcknowledgmentDefinitionCommandHandler(
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
        CreateAcknowledgmentDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = new AcknowledgmentDefinition(
            title: request.Title,
            ownerDepartment: request.OwnerDepartment,
            defaultActionType: request.DefaultActionType,
            description: request.Description,
            createdBy: _currentUser.Username);

        await _acknowledgments.AddAsync(definition, cancellationToken).ConfigureAwait(false);
        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.DefinitionCreated(definition.Id, definition.Title, _currentUser.Username);

        return _mapper.Map<AcknowledgmentDefinitionDetailDto>(definition);
    }
}
