using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using Eap.Domain.Policy;
using MediatR;

namespace Eap.Application.Policies.Commands.CreatePolicy;

public sealed class CreatePolicyCommandHandler : IRequestHandler<CreatePolicyCommand, PolicyDetailDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public CreatePolicyCommandHandler(
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

    public async Task<PolicyDetailDto> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.PolicyCode.Trim();

        if (await _policies.CodeExistsAsync(normalizedCode, null, cancellationToken).ConfigureAwait(false))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(CreatePolicyCommand.PolicyCode),
                    $"A policy with code '{normalizedCode}' already exists."),
            });
        }

        var policy = new Policy(
            policyCode: normalizedCode,
            title: request.Title,
            ownerDepartment: request.OwnerDepartment,
            category: request.Category,
            description: request.Description,
            createdBy: _currentUser.Username);

        await _policies.AddAsync(policy, cancellationToken).ConfigureAwait(false);
        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyCreated(policy.Id, policy.PolicyCode, _currentUser.Username);

        return _mapper.Map<PolicyDetailDto>(policy);
    }
}
