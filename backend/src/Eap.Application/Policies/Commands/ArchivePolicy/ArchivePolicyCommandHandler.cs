using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.ArchivePolicy;

public sealed class ArchivePolicyCommandHandler : IRequestHandler<ArchivePolicyCommand, PolicyDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public ArchivePolicyCommandHandler(
        IPolicyRepository policies,
        IPolicyAuditLogger audit,
        ICurrentUser currentUser,
        TimeProvider clock,
        IMapper mapper)
    {
        _policies = policies;
        _audit = audit;
        _currentUser = currentUser;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<PolicyDetailDto> Handle(ArchivePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        policy.Archive(_currentUser.Username, _clock.GetUtcNow());

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyArchived(policy.Id, policy.PolicyCode, _currentUser.Username);

        return _mapper.Map<PolicyDetailDto>(policy);
    }
}
