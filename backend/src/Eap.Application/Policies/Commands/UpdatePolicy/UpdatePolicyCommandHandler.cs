using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.UpdatePolicy;

public sealed class UpdatePolicyCommandHandler : IRequestHandler<UpdatePolicyCommand, PolicyDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UpdatePolicyCommandHandler(
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

    public async Task<PolicyDetailDto> Handle(UpdatePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        policy.UpdateMetadata(
            title: request.Title,
            ownerDepartment: request.OwnerDepartment,
            category: request.Category,
            description: request.Description,
            updatedBy: _currentUser.Username);

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyMetadataUpdated(policy.Id, policy.PolicyCode, _currentUser.Username);

        return _mapper.Map<PolicyDetailDto>(policy);
    }
}
