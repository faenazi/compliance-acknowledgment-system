using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Commands.SubmitForm;

/// <summary>
/// Creates a user submission against a published form-based disclosure version
/// (BR-078, BR-091). Validates the payload against the form definition (BR-074..BR-077),
/// stores a form snapshot (BR-079), and optionally flattens field values (§8.2).
/// </summary>
public sealed record SubmitFormCommand(
    Guid DefinitionId,
    Guid VersionId,
    string SubmissionJson) : IRequest<UserSubmissionDetailDto>;
