using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Domain.Policy;
using FluentValidation.Results;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.PublishAcknowledgmentVersion;

public sealed class PublishAcknowledgmentVersionCommandHandler
    : IRequestHandler<PublishAcknowledgmentVersionCommand, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IPolicyRepository _policies;
    private readonly IAcknowledgmentAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;
    private readonly IMapper _mapper;

    public PublishAcknowledgmentVersionCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IPolicyRepository policies,
        IAcknowledgmentAuditLogger audit,
        ICurrentUser currentUser,
        TimeProvider clock,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _policies = policies;
        _audit = audit;
        _currentUser = currentUser;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentVersionDetailDto> Handle(
        PublishAcknowledgmentVersionCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var target = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        // LR-001: re-verify the linked policy version is still published at publish time.
        var policyVersion = await _policies
            .FindVersionLookupAsync(target.PolicyVersionId, cancellationToken)
            .ConfigureAwait(false);

        if (policyVersion is null || policyVersion.Status != PolicyVersionStatus.Published)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(PublishAcknowledgmentVersionCommand.VersionId),
                    "Cannot publish: the linked policy version is no longer published."),
            });
        }

        // Aggregate enforces "one active published version" and supersedes the
        // previous published version atomically. The DB also has a filtered
        // unique index as defence-in-depth.
        definition.PublishVersion(request.VersionId, _currentUser.Username, _clock.GetUtcNow());

        await _acknowledgments.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var published = definition.Versions.Single(v => v.Id == request.VersionId);
        _audit.VersionPublished(definition.Id, published.Id, published.VersionNumber, _currentUser.Username);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(published);
    }
}
