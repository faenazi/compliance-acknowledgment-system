using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.ArchivePolicyVersion;

public sealed class ArchivePolicyVersionCommandHandler
    : IRequestHandler<ArchivePolicyVersionCommand, PolicyVersionDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public ArchivePolicyVersionCommandHandler(
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

    public async Task<PolicyVersionDetailDto> Handle(
        ArchivePolicyVersionCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        policy.ArchiveVersion(request.VersionId, _currentUser.Username, _clock.GetUtcNow());

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var version = policy.Versions.Single(v => v.Id == request.VersionId);
        _audit.PolicyVersionArchived(policy.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<PolicyVersionDetailDto>(version);
    }
}
