using System.Text.Json;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Forms.Services;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Forms;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.UserPortal.Commands.SubmitDisclosure;

public sealed class SubmitDisclosureCommandHandler
    : IRequestHandler<SubmitDisclosureCommand, SubmissionResultDto>
{
    private readonly IUserPortalRepository _portal;
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IFormAuditLogger _audit;
    private readonly ICurrentUser _currentUser;

    public SubmitDisclosureCommandHandler(
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
        SubmitDisclosureCommand request,
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

        // Load the acknowledgment version and verify it's form-based (repository eagerly loads FormDefinition)
        var definition = await _acknowledgments
            .FindDefinitionByVersionIdAsync(requirement.AcknowledgmentVersionId, cancellationToken)
            ?? throw new NotFoundException("AcknowledgmentDefinition", requirement.AcknowledgmentVersionId);

        var ackVersion = definition.Versions.Single(v => v.Id == requirement.AcknowledgmentVersionId);

        if (ackVersion.ActionType != ActionType.FormBasedDisclosure)
        {
            throw new InvalidOperationException(
                "This requirement does not require a form-based submission.");
        }

        var formDef = ackVersion.FormDefinition
            ?? throw new InvalidOperationException(
                "Published form-based disclosure is missing its form definition.");

        // Validate submission against the form definition (BR-074..BR-077)
        var validationErrors = SubmissionValidator.Validate(formDef, request.SubmissionJson);
        if (validationErrors.Count > 0)
        {
            var failures = validationErrors
                .Select(e => new FluentValidation.Results.ValidationFailure(e.FieldKey, e.Message))
                .ToList();

            throw new Common.Exceptions.ValidationException(failures);
        }

        // Take form definition snapshot (BR-079)
        var snapshot = formDef.TakeSnapshot();
        var snapshotJson = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        });

        // Determine late submission
        var isLate = requirement.Status == UserActionRequirementStatus.Overdue;

        // Create submission using the form-based constructor
        var submission = new UserSubmission(
            userId: userId,
            acknowledgmentVersionId: ackVersion.Id,
            formDefinitionId: formDef.Id,
            submissionJson: request.SubmissionJson,
            formDefinitionSnapshotJson: snapshotJson,
            submittedBy: _currentUser.Username);

        // Link to requirement
        submission.LinkToRequirement(requirement.Id, isLate);

        // Flatten field values for reporting (§8.2)
        FlattenFieldValues(submission, formDef, request.SubmissionJson);

        // Mark requirement as completed (BR-102)
        var now = DateTimeOffset.UtcNow;
        requirement.MarkCompleted(now, _currentUser.Username);

        await _portal.AddSubmissionAsync(submission, cancellationToken);
        await _portal.SaveChangesAsync(cancellationToken);

        _audit.FormSubmissionCreated(submission.Id, userId, ackVersion.Id, formDef.Id, _currentUser.Username);

        return new SubmissionResultDto
        {
            SubmissionId = submission.Id,
            RequirementId = requirement.Id,
            SubmittedAtUtc = submission.SubmittedAtUtc,
            RequirementStatus = requirement.Status,
            IsLateSubmission = isLate,
        };
    }

    private static void FlattenFieldValues(
        UserSubmission submission,
        FormDefinition formDef,
        string submissionJson)
    {
        Dictionary<string, JsonElement>? values;
        try
        {
            values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(submissionJson);
        }
        catch
        {
            return;
        }

        if (values is null) return;

        foreach (var field in formDef.Fields.Where(f => FormFieldTypes.CollectsValue(f.FieldType)))
        {
            if (!values.TryGetValue(field.FieldKey, out var element) ||
                element.ValueKind == JsonValueKind.Undefined ||
                element.ValueKind == JsonValueKind.Null)
                continue;

            string? valueText = null;
            decimal? valueNumber = null;
            DateOnly? valueDate = null;
            bool? valueBoolean = null;
            string? valueJson = null;

            switch (field.FieldType)
            {
                case FormFieldType.ShortText:
                case FormFieldType.LongText:
                case FormFieldType.Email:
                case FormFieldType.PhoneNumber:
                case FormFieldType.RadioGroup:
                case FormFieldType.Dropdown:
                case FormFieldType.FileUpload:
                    valueText = element.ValueKind == JsonValueKind.String ? element.GetString() : element.ToString();
                    break;

                case FormFieldType.Number:
                case FormFieldType.Decimal:
                    if (element.TryGetDecimal(out var dec)) valueNumber = dec;
                    break;

                case FormFieldType.Date:
                    if (element.ValueKind == JsonValueKind.String && DateOnly.TryParse(element.GetString(), out var d))
                        valueDate = d;
                    break;

                case FormFieldType.Checkbox:
                case FormFieldType.YesNo:
                    valueBoolean = element.ValueKind == JsonValueKind.True;
                    break;

                case FormFieldType.MultiSelect:
                    valueJson = element.GetRawText();
                    break;
            }

            submission.AddFieldValue(new UserSubmissionFieldValue(
                userSubmissionId: submission.Id,
                fieldKey: field.FieldKey,
                fieldLabel: field.Label,
                fieldType: field.FieldType,
                valueText: valueText,
                valueNumber: valueNumber,
                valueDate: valueDate,
                valueBoolean: valueBoolean,
                valueJson: valueJson));
        }
    }
}
