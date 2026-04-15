using Eap.Domain.Common;

namespace Eap.Domain.Acknowledgment;

/// <summary>
/// Immutable-once-published snapshot of an <see cref="AcknowledgmentDefinition"/>
/// (docs/05-data/conceptual-data-model.md §6.2,
/// docs/03-functional-requirements/lifecycle-models.md §4.2).
///
/// Each version is linked to exactly one <c>PolicyVersion</c> (LR-001/CDM-001).
/// Recurrence and audience are intentionally deferred to Sprint 4 and are NOT
/// modelled here.
/// </summary>
public sealed class AcknowledgmentVersion : AuditableEntity
{
    // EF Core
    private AcknowledgmentVersion() { }

    internal AcknowledgmentVersion(
        Guid acknowledgmentDefinitionId,
        int versionNumber,
        Guid policyVersionId,
        ActionType actionType,
        string? versionLabel,
        string? summary,
        string? commitmentText,
        DateOnly? startDate,
        DateOnly? dueDate,
        string? createdBy)
    {
        if (acknowledgmentDefinitionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Acknowledgment definition id is required.",
                nameof(acknowledgmentDefinitionId));
        }

        if (versionNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(versionNumber), "Version number must be positive.");
        }

        if (policyVersionId == Guid.Empty)
        {
            throw new ArgumentException(
                "A policy version id is required (LR-001).",
                nameof(policyVersionId));
        }

        ValidateDateWindow(startDate, dueDate);

        AcknowledgmentDefinitionId = acknowledgmentDefinitionId;
        VersionNumber = versionNumber;
        PolicyVersionId = policyVersionId;
        ActionType = actionType;
        VersionLabel = versionLabel?.Trim();
        Summary = summary?.Trim();
        CommitmentText = commitmentText?.Trim();
        StartDate = startDate;
        DueDate = dueDate;
        Status = AcknowledgmentVersionStatus.Draft;
        CreatedBy = createdBy?.Trim();
    }

    public Guid AcknowledgmentDefinitionId { get; private set; }

    public int VersionNumber { get; private set; }

    public string? VersionLabel { get; private set; }

    public Guid PolicyVersionId { get; private set; }

    public ActionType ActionType { get; private set; }

    public string? Summary { get; private set; }

    /// <summary>Free-text commitment shown to the user when
    /// <see cref="ActionType"/> is <see cref="ActionType.AcknowledgmentWithCommitment"/>.</summary>
    public string? CommitmentText { get; private set; }

    public DateOnly? StartDate { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public AcknowledgmentVersionStatus Status { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public string? PublishedBy { get; private set; }

    public DateTimeOffset? ArchivedAtUtc { get; private set; }

    public string? ArchivedBy { get; private set; }

    public Guid? SupersededByAcknowledgmentVersionId { get; private set; }

    /// <summary>Edits to metadata are only allowed while the version is still a draft.</summary>
    public void UpdateDraftMetadata(
        Guid policyVersionId,
        ActionType actionType,
        string? versionLabel,
        string? summary,
        string? commitmentText,
        DateOnly? startDate,
        DateOnly? dueDate,
        string? updatedBy)
    {
        EnsureDraft("update");

        if (policyVersionId == Guid.Empty)
        {
            throw new ArgumentException(
                "A policy version id is required (LR-001).",
                nameof(policyVersionId));
        }

        ValidateDateWindow(startDate, dueDate);

        PolicyVersionId = policyVersionId;
        ActionType = actionType;
        VersionLabel = versionLabel?.Trim();
        Summary = summary?.Trim();
        CommitmentText = commitmentText?.Trim();
        StartDate = startDate;
        DueDate = dueDate;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }

    /// <summary>Publishes the version. Callers enforce the "one published version per
    /// definition" rule at the aggregate level via <see cref="AcknowledgmentDefinition.PublishVersion"/>.</summary>
    internal void MarkPublished(string? publishedBy, DateTimeOffset whenUtc)
    {
        EnsureDraft("publish");

        if (ActionType == ActionType.AcknowledgmentWithCommitment && string.IsNullOrWhiteSpace(CommitmentText))
        {
            throw new InvalidOperationException(
                "Cannot publish an 'Acknowledgment with Commitment' version without commitment text.");
        }

        Status = AcknowledgmentVersionStatus.Published;
        PublishedAtUtc = whenUtc;
        PublishedBy = publishedBy?.Trim();
        UpdatedAtUtc = whenUtc;
        UpdatedBy = PublishedBy;
    }

    internal void MarkSuperseded(Guid newVersionId, DateTimeOffset whenUtc)
    {
        if (Status != AcknowledgmentVersionStatus.Published)
        {
            throw new InvalidOperationException("Only a published version can be superseded.");
        }

        if (newVersionId == Guid.Empty || newVersionId == Id)
        {
            throw new ArgumentException("A valid successor version id is required.", nameof(newVersionId));
        }

        Status = AcknowledgmentVersionStatus.Superseded;
        SupersededByAcknowledgmentVersionId = newVersionId;
        UpdatedAtUtc = whenUtc;
    }

    internal void MarkArchived(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == AcknowledgmentVersionStatus.Archived)
        {
            return;
        }

        if (Status == AcknowledgmentVersionStatus.Published)
        {
            throw new InvalidOperationException(
                "Archive the definition or publish a successor first; a currently published version cannot be archived directly.");
        }

        ApplyArchive(archivedBy, whenUtc);
    }

    /// <summary>Archive path used when the parent definition itself is being archived.</summary>
    internal void ArchiveAsPartOfDefinitionArchive(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == AcknowledgmentVersionStatus.Archived)
        {
            return;
        }

        ApplyArchive(archivedBy, whenUtc);
    }

    private void ApplyArchive(string? archivedBy, DateTimeOffset whenUtc)
    {
        Status = AcknowledgmentVersionStatus.Archived;
        ArchivedAtUtc = whenUtc;
        ArchivedBy = archivedBy?.Trim();
        UpdatedAtUtc = whenUtc;
    }

    private void EnsureDraft(string attemptedOperation)
    {
        if (Status != AcknowledgmentVersionStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Cannot {attemptedOperation} acknowledgment version {Id} because it is not a draft.");
        }
    }

    private static void ValidateDateWindow(DateOnly? startDate, DateOnly? dueDate)
    {
        if (startDate is not null && dueDate is not null && dueDate < startDate)
        {
            throw new ArgumentException("Due date cannot be earlier than start date.", nameof(dueDate));
        }
    }
}
