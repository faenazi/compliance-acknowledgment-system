using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Commands.SubmitDisclosure;

/// <summary>
/// Submits a form-based disclosure for the current user's requirement.
/// Validates the form payload against the form definition, creates a
/// UserSubmission with snapshot, and marks the requirement as Completed (BR-102).
/// </summary>
public sealed record SubmitDisclosureCommand(
    Guid RequirementId,
    string SubmissionJson) : IRequest<SubmissionResultDto>;
