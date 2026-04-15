using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.UpdatePolicyVersionDraft;

public sealed class UpdatePolicyVersionDraftCommandHandler
    : IRequestHandler<UpdatePolicyVersionDraftCommand, PolicyVersionDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UpdatePolicyVersionDraftCommandHandler(
        IPolicyRepository policies,
        IPolicyAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _policies = policies;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PolicyVersionDetailDto> Handle(
        UpdatePolicyVersionDraftCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        var version = policy.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("PolicyVersion", request.VersionId);

        version.UpdateDraftMetadata(
            versionLabel: request.VersionLabel,
            effectiveDate: request.EffectiveDate,
            summary: request.Summary,
            updatedBy: _currentUser.Username);

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyVersionUpdated(policy.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<PolicyVersionDetailDto>(version);
    }
}
