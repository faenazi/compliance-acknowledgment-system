using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.UploadPolicyDocument;

/// <summary>
/// Attaches (or replaces) the document for a draft version.
/// <para>
/// Callers are responsible for disposing <see cref="Content"/> after sending
/// the request; the handler only reads from it.
/// </para>
/// </summary>
public sealed record UploadPolicyDocumentCommand(
    Guid PolicyId,
    Guid VersionId,
    string FileName,
    string ContentType,
    long FileSize,
    Stream Content) : IRequest<PolicyDocumentDto>;
