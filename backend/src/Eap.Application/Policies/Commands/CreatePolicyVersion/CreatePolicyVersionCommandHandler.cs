using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.CreatePolicyVersion;

public sealed class CreatePolicyVersionCommandHandler : IRequestHandler<CreatePolicyVersionCommand, PolicyVersionDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public CreatePolicyVersionCommandHandler(
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
        CreatePolicyVersionCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        // Monotonic per-policy version numbers — next = max + 1. Uniqueness is also
        // enforced at the DB layer via a composite unique index.
        var nextNumber = await _policies
            .GetMaxVersionNumberAsync(policy.Id, cancellationToken)
            .ConfigureAwait(false) + 1;

        var version = policy.CreateDraftVersion(
            nextVersionNumber: nextNumber,
            versionLabel: request.VersionLabel,
            effectiveDate: request.EffectiveDate,
            summary: request.Summary,
            createdBy: _currentUser.Username);

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyVersionCreated(policy.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<PolicyVersionDetailDto>(version);
    }
}
