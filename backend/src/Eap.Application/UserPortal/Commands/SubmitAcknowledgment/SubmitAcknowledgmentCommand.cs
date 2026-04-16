using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Commands.SubmitAcknowledgment;

/// <summary>
/// Submits a simple acknowledgment or commitment acknowledgment for the
/// current user's requirement. Creates a UserSubmission record and marks
/// the UserActionRequirement as Completed (BR-102).
/// </summary>
public sealed record SubmitAcknowledgmentCommand(Guid RequirementId) : IRequest<SubmissionResultDto>;
