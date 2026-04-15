using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentVersion;

/// <summary>Archives a draft or superseded acknowledgment version.</summary>
public sealed record ArchiveAcknowledgmentVersionCommand(Guid DefinitionId, Guid VersionId)
    : IRequest<AcknowledgmentVersionDetailDto>;
