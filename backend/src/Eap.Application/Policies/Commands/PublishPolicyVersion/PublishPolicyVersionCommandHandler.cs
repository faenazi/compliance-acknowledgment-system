using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.PublishPolicyVersion;

public sealed class PublishPolicyVersionCommandHandler
    : IRequestHandler<PublishPolicyVersionCommand, PolicyVersionDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public PublishPolicyVersionCommandHandler(
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
        PublishPolicyVersionCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        // Enforces BR-010 (document required) and BR-011 (single published version)
        // inside the aggregate. The DB additionally has a filtered unique index as
        // a defence-in-depth guarantee.
        policy.PublishVersion(request.VersionId, _currentUser.Username, _clock.GetUtcNow());

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var published = policy.Versions.Single(v => v.Id == request.VersionId);
        _audit.PolicyVersionPublished(policy.Id, published.Id, published.VersionNumber, _currentUser.Username);

        return _mapper.Map<PolicyVersionDetailDto>(published);
    }
}
