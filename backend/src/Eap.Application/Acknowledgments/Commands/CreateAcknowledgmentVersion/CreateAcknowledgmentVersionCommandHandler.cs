using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Domain.Policy;
using FluentValidation.Results;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentVersion;

public sealed class CreateAcknowledgmentVersionCommandHandler
    : IRequestHandler<CreateAcknowledgmentVersionCommand, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IPolicyRepository _policies;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public CreateAcknowledgmentVersionCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IPolicyRepository policies,
        IAcknowledgmentAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _policies = policies;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentVersionDetailDto> Handle(
        CreateAcknowledgmentVersionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var policyVersion = await _policies
            .FindVersionLookupAsync(request.PolicyVersionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(CreateAcknowledgmentVersionCommand.PolicyVersionId),
                    $"Policy version '{request.PolicyVersionId}' was not found."),
            });

        // LR-001: ack versions may only link to a published policy version.
        if (policyVersion.Status != PolicyVersionStatus.Published)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(CreateAcknowledgmentVersionCommand.PolicyVersionId),
                    "The linked policy version must be published before an acknowledgment version can reference it."),
            });
        }

        var nextNumber = await _acknowledgments
            .GetMaxVersionNumberAsync(definition.Id, cancellationToken)
            .ConfigureAwait(false) + 1;

        var version = definition.CreateDraftVersion(
            nextVersionNumber: nextNumber,
            policyVersionId: request.PolicyVersionId,
            actionType: request.ActionType,
            recurrenceModel: request.RecurrenceModel,
            versionLabel: request.VersionLabel,
            summary: request.Summary,
            commitmentText: request.CommitmentText,
            startDate: request.StartDate,
            dueDate: request.DueDate,
            createdBy: _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.VersionCreated(definition.Id, version.Id, version.VersionNumber, version.PolicyVersionId, _currentUser.Username);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(version);
    }
}
