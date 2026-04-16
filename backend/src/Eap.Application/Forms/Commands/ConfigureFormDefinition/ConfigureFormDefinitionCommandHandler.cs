using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Forms.Models;
using Eap.Domain.Forms;
using MediatR;

namespace Eap.Application.Forms.Commands.ConfigureFormDefinition;

public sealed class ConfigureFormDefinitionCommandHandler
    : IRequestHandler<ConfigureFormDefinitionCommand, FormDefinitionDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IFormAuditLogger _audit;
    private readonly Eap.Application.Identity.Abstractions.ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ConfigureFormDefinitionCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IFormAuditLogger audit,
        Eap.Application.Identity.Abstractions.ICurrentUser currentUser,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<FormDefinitionDto> Handle(
        ConfigureFormDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        var formDef = version.ConfigureFormDefinition(_currentUser.Username);

        var inputs = request.Fields.Select(f => new FormFieldInput(
            FieldKey: f.FieldKey,
            Label: f.Label,
            FieldType: f.FieldType,
            IsRequired: f.IsRequired,
            SectionKey: f.SectionKey,
            HelpText: f.HelpText,
            Placeholder: f.Placeholder,
            DisplayText: f.DisplayText,
            Options: f.Options?.Select(o => new FieldOption(o.Value, o.Label)).ToList()
        )).ToList();

        formDef.ReplaceFields(inputs, _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.FormDefinitionConfigured(
            definition.Id, version.Id, formDef.Id, formDef.Fields.Count, _currentUser.Username);

        return _mapper.Map<FormDefinitionDto>(formDef);
    }
}
