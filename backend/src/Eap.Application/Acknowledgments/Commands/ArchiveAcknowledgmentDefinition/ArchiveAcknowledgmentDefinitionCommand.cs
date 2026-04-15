using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentDefinition;

/// <summary>Archives an acknowledgment definition and any currently published version.</summary>
public sealed record ArchiveAcknowledgmentDefinitionCommand(Guid DefinitionId)
    : IRequest<AcknowledgmentDefinitionDetailDto>;
