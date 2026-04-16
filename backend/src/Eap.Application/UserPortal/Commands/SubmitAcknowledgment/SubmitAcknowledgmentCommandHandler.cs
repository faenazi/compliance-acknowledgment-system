using System.Text.Json;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Forms;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.UserPortal.Commands.SubmitAcknowledgment;

public sealed class SubmitAcknowledgmentCommandHandler
    : IRequestHandler<SubmitAcknowledgmentCommand, SubmissionResultDto>
{
    private readonly IUserPortalRepository _portal;
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IFormAuditLogger _audit;
    private readonly ICurrentUser _currentUser;

    public SubmitAcknowledgmentCommandHandler(
        IUserPortalRepository portal,
        IAcknowledgmentRepository acknowledgments,
        IFormAuditLogger audit,
        ICurrentUser currentUser)
    {
        _portal = portal;
        _acknowledgments = acknowledgments;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<SubmissionResultDto> Handle(
        SubmitAcknowledgmentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        // Find the open requirement belonging to this user
        var requirement = await _portal.FindOpenRequirementAsync(userId, request.RequirementId, cancellationToken)
            ?? throw new NotFoundException("UserActionRequirement", request.RequirementId);

        // Check for duplicate submission (BR-036)
        if (await _portal.HasSubmissionForRequirementAsync(userId, request.RequirementId, cancellationToken))
        {
            throw new InvalidOperationException(
                "A submission already exists for this requirement (BR-036).");
        }

        // Load the acknowledgment version to validate action type
        var definition = await _acknowledgments
            .FindDefinitionByVersionIdAsync(requirement.AcknowledgmentVersionId, cancellationToken)
            ?? throw new NotFoundException("AcknowledgmentDefinition", requirement.AcknowledgmentVersionId);

        var ackVersion = definition.Versions.Single(v => v.Id == requirement.AcknowledgmentVersionId);

        if (ackVersion.ActionType == ActionType.FormBasedDisclosure)
        {
            throw new InvalidOperationException(
                "Form-based disclosures must be submitted through the disclosure endpoint.");
        }

        // Build minimal submission JSON
        var submissionData = new
        {
            type = ackVersion.ActionType == ActionType.SimpleAcknowledgment ? "simple" : "commitment",
            confirmed = true,
            timestamp = DateTimeOffset.UtcNow,
        };
        var submissionJson = JsonSerializer.Serialize(submissionData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        // Determine late submission
        var isLate = requirement.Status == UserActionRequirementStatus.Overdue;

        // Create submission
        var submission = new UserSubmission(
            userId: userId,
            acknowledgmentVersionId: ackVersion.Id,
            userActionRequirementId: requirement.Id,
            submissionJson: submissionJson,
            isLateSubmission: isLate,
            submittedBy: _currentUser.Username);

        // Mark requirement as completed (BR-102)
        var now = DateTimeOffset.UtcNow;
        requirement.MarkCompleted(now, _currentUser.Username);

        await _portal.AddSubmissionAsync(submission, cancellationToken);
        await _portal.SaveChangesAsync(cancellationToken);

        _audit.FormSubmissionCreated(submission.Id, userId, ackVersion.Id, Guid.Empty, _currentUser.Username);

        return new SubmissionResultDto
        {
            SubmissionId = submission.Id,
            RequirementId = requirement.Id,
            SubmittedAtUtc = submission.SubmittedAtUtc,
            RequirementStatus = requirement.Status,
            IsLateSubmission = isLate,
        };
    }
}
