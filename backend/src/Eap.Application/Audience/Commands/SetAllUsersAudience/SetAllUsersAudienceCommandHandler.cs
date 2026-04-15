using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Audience.Abstractions;
using Eap.Application.Audience.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Audience.Commands.SetAllUsersAudience;

public sealed class SetAllUsersAudienceCommandHandler
    : IRequestHandler<SetAllUsersAudienceCommand, AudienceDefinitionDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAudienceAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public SetAllUsersAudienceCommandHandler(
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
        SetAllUsersAudienceCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        var audience = version.ConfigureAudience(_currentUser.Username);
        audience.SetAllUsers(_currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.AudienceConfigured(
            definition.Id, version.Id, audience.Id,
            inclusionCount: 1, exclusionCount: 0,
            _currentUser.Username);

        return _mapper.Map<AudienceDefinitionDto>(audience);
    }
}
