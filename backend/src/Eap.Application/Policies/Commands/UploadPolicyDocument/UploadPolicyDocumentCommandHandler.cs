using AutoMapper;
using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using Eap.Domain.Policy;
using MediatR;

namespace Eap.Application.Policies.Commands.UploadPolicyDocument;

public sealed class UploadPolicyDocumentCommandHandler
    : IRequestHandler<UploadPolicyDocumentCommand, PolicyDocumentDto>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyDocumentStorage _storage;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UploadPolicyDocumentCommandHandler(
        IPolicyRepository policies,
        IPolicyDocumentStorage storage,
        IPolicyAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _policies = policies;
        _storage = storage;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PolicyDocumentDto> Handle(
        UploadPolicyDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        var version = policy.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("PolicyVersion", request.VersionId);

        if (version.Status != PolicyVersionStatus.Draft)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(UploadPolicyDocumentCommand.VersionId),
                    "Documents can only be uploaded to draft versions (BR-003)."),
            });
        }

        // Persist the bytes first. If the domain rejects the replacement, we orphan
        // the blob — captured in the audit stream and cleaned up by a later janitor
        // job; trading eventual consistency for not losing uploads mid-flight.
        var stored = await _storage
            .StoreAsync(
                policyId: policy.Id,
                policyVersionId: version.Id,
                fileName: request.FileName,
                contentType: request.ContentType,
                content: request.Content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var document = new PolicyDocument(
            policyVersionId: version.Id,
            fileName: request.FileName,
            contentType: request.ContentType,
            fileSize: stored.FileSize,
            storageReference: stored.StorageReference,
            uploadedBy: _currentUser.Username);

        version.AttachDocument(document);

        await _policies.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.PolicyDocumentUploaded(
            policy.Id,
            version.Id,
            document.Id,
            document.FileName,
            document.FileSize,
            _currentUser.Username);

        return _mapper.Map<PolicyDocumentDto>(document);
    }
}
