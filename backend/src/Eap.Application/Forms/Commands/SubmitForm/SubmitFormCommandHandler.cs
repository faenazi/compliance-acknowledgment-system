using System.Text.Json;
using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Forms.Models;
using Eap.Application.Forms.Services;
using Eap.Application.Identity.Abstractions;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Forms;
using MediatR;

namespace Eap.Application.Forms.Commands.SubmitForm;

public sealed class SubmitFormCommandHandler
    : IRequestHandler<SubmitFormCommand, UserSubmissionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IUserSubmissionRepository _submissions;
    private readonly IFormAuditLogger _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public SubmitFormCommandHandler(
        IAcknowledgmentRepository acknowledgments,
        IUserSubmissionRepository submissions,
        IFormAuditLogger audit,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _submissions = submissions;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<UserSubmissionDetailDto> Handle(
        SubmitFormCommand request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        if (version.Status != AcknowledgmentVersionStatus.Published)
        {
            throw new InvalidOperationException(
                "Submissions can only be made against a published version (BR-083).");
        }

        if (version.ActionType != ActionType.FormBasedDisclosure)
        {
            throw new InvalidOperationException(
                "This version does not require a form-based submission (BR-023).");
        }

        var formDef = version.FormDefinition
            ?? throw new InvalidOperationException(
                "Published form-based disclosure is missing its form definition — data integrity issue.");

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

        // Resolve user id
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required for submission.");

        var submission = new UserSubmission(
            userId: userId,
            acknowledgmentVersionId: version.Id,
            formDefinitionId: formDef.Id,
            submissionJson: request.SubmissionJson,
            formDefinitionSnapshotJson: snapshotJson,
            submittedBy: _currentUser.Username);

        // Optional field-value flattening for reporting (§8.2)
        FlattenFieldValues(submission, formDef, request.SubmissionJson);

        await _submissions.AddAsync(submission, cancellationToken).ConfigureAwait(false);
        await _submissions.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.FormSubmissionCreated(submission.Id, userId, version.Id, formDef.Id, _currentUser.Username);

        return _mapper.Map<UserSubmissionDetailDto>(submission);
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
