using Eap.Application.Common.Exceptions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Policies.Abstractions;
using MediatR;

namespace Eap.Application.Policies.Queries.DownloadPolicyDocument;

public sealed class DownloadPolicyDocumentQueryHandler
    : IRequestHandler<DownloadPolicyDocumentQuery, PolicyDocumentDownloadResult>
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyDocumentStorage _storage;
    private readonly IPolicyAuditLogger _audit;
    private readonly ICurrentUser _currentUser;

    public DownloadPolicyDocumentQueryHandler(
        IPolicyRepository policies,
        IPolicyDocumentStorage storage,
        IPolicyAuditLogger audit,
        ICurrentUser currentUser)
    {
        _policies = policies;
        _storage = storage;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<PolicyDocumentDownloadResult> Handle(
        DownloadPolicyDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await _policies.FindByIdAsync(request.PolicyId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("Policy", request.PolicyId);

        var version = policy.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("PolicyVersion", request.VersionId);

        var document = version.Document
            ?? throw new NotFoundException("PolicyDocument", request.VersionId);

        var stream = await _storage
            .OpenReadAsync(document.StorageReference, cancellationToken)
            .ConfigureAwait(false);

        _audit.PolicyDocumentDownloaded(
            policy.Id,
            version.Id,
            document.Id,
            document.FileName,
            _currentUser.Username);

        return new PolicyDocumentDownloadResult
        {
            Content = stream,
            FileName = document.FileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
        };
    }
}
