using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.PublishAcknowledgmentVersion;

/// <summary>Transitions a draft acknowledgment version to Published and supersedes
/// the previously published version (if any) atomically.</summary>
public sealed record PublishAcknowledgmentVersionCommand(Guid DefinitionId, Guid VersionId)
    : IRequest<AcknowledgmentVersionDetailDto>;
