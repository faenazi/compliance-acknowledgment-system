using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Domain.Policy;
using FluentValidation.Results;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentVersionDraft;

public sealed class UpdateAcknowledgmentVersionDraftCommandHandler
    : IRequestHandler<UpdateAcknowledgmentVersionDraftCommand, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IPolicyRepository _policies;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UpdateAcknowledgmentVersionDraftCommandHandler(
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
        UpdateAcknowledgmentVersionDraftCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        var policyVersion = await _policies
            .FindVersionLookupAsync(request.PolicyVersionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(UpdateAcknowledgmentVersionDraftCommand.PolicyVersionId),
                    $"Policy version '{request.PolicyVersionId}' was not found."),
            });

        if (policyVersion.Status != PolicyVersionStatus.Published)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(UpdateAcknowledgmentVersionDraftCommand.PolicyVersionId),
                    "The linked policy version must be published before an acknowledgment version can reference it."),
            });
        }

        version.UpdateDraftMetadata(
            policyVersionId: request.PolicyVersionId,
            actionType: request.ActionType,
            versionLabel: request.VersionLabel,
            summary: request.Summary,
            commitmentText: request.CommitmentText,
            startDate: request.StartDate,
            dueDate: request.DueDate,
            updatedBy: _currentUser.Username);

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.VersionUpdated(definition.Id, version.Id, version.VersionNumber, _currentUser.Username);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(version);
    }
}
