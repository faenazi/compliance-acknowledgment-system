using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Audience.Abstractions;
using Eap.Application.Audience.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Domain.Audience;
using MediatR;

namespace Eap.Application.Audience.Commands.ConfigureAudienceInclusion;

public sealed class ConfigureAudienceInclusionCommandHandler
    : IRequestHandler<ConfigureAudienceInclusionCommand, AudienceDefinitionDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAudienceAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ConfigureAudienceInclusionCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IAudienceAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<AudienceDefinitionDto> Handle(
        ConfigureAudienceInclusionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        var audience = version.ConfigureAudience(_currentUser.Username);

        var inputs = request.Rules.Select(r =>
            new AudienceRuleInput(r.RuleType, r.RuleValue, IsExclusion: false)).ToList();

        audience.ReplaceInclusionRules(inputs, _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.InclusionRulesReplaced(
            definition.Id, version.Id, audience.Id, inputs.Count, _currentUser.Username);

        return _mapper.Map<AudienceDefinitionDto>(audience);
    }
}
