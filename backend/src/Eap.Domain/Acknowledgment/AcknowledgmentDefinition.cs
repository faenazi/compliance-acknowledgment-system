using Eap.Domain.Common;

namespace Eap.Domain.Acknowledgment;

/// <summary>
/// Master acknowledgment record (docs/05-data/conceptual-data-model.md §6.1).
/// Aggregates many <see cref="AcknowledgmentVersion"/> snapshots over time.
/// The master row carries long-lived metadata (title, owning department, action
/// type defaults); versioned business content — policy linkage, commitment text,
/// schedule window — lives on the version.
/// </summary>
public sealed class AcknowledgmentDefinition : AuditableEntity
{
    private readonly List<AcknowledgmentVersion> _versions = new();

    // EF Core
    private AcknowledgmentDefinition() { }

    public AcknowledgmentDefinition(
        string title,
        string ownerDepartment,
        ActionType defaultActionType,
        string? description,
        string? createdBy)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Acknowledgment title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(ownerDepartment))
        {
            throw new ArgumentException("Owner department is required.", nameof(ownerDepartment));
        }

        Title = title.Trim();
        OwnerDepartment = ownerDepartment.Trim();
        DefaultActionType = defaultActionType;
        Description = description?.Trim();
        Status = AcknowledgmentStatus.Draft;
        CreatedBy = createdBy?.Trim();
    }

    public string Title { get; private set; } = default!;

    public string OwnerDepartment { get; private set; } = default!;

    public ActionType DefaultActionType { get; private set; }

    public string? Description { get; private set; }

    public AcknowledgmentStatus Status { get; private set; }

    public Guid? CurrentAcknowledgmentVersionId { get; private set; }

    public IReadOnlyCollection<AcknowledgmentVersion> Versions => _versions.AsReadOnly();

    public void UpdateMetadata(
        string title,
        string ownerDepartment,
        ActionType defaultActionType,
        string? description,
        string? updatedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Acknowledgment title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(ownerDepartment))
        {
            throw new ArgumentException("Owner department is required.", nameof(ownerDepartment));
        }

        Title = title.Trim();
        OwnerDepartment = ownerDepartment.Trim();
        DefaultActionType = defaultActionType;
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }

    /// <summary>
    /// Creates a new draft version. Caller supplies the monotonic next version
    /// number (queried by the repository) and the target <paramref name="policyVersionId"/>
    /// already validated to be a published policy version (LR-001).
    /// </summary>
    public AcknowledgmentVersion CreateDraftVersion(
        int nextVersionNumber,
        Guid policyVersionId,
        ActionType actionType,
        RecurrenceModel recurrenceModel,
        string? versionLabel,
        string? summary,
        string? commitmentText,
        DateOnly? startDate,
        DateOnly? dueDate,
        string? createdBy)
    {
        if (Status == AcknowledgmentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Cannot create a draft on an archived acknowledgment definition. Reactivate it first.");
        }

        var version = new AcknowledgmentVersion(
            acknowledgmentDefinitionId: Id,
            versionNumber: nextVersionNumber,
            policyVersionId: policyVersionId,
            actionType: actionType,
            recurrenceModel: recurrenceModel,
            versionLabel: versionLabel,
            summary: summary,
            commitmentText: commitmentText,
            startDate: startDate,
            dueDate: dueDate,
            createdBy: createdBy);

        _versions.Add(version);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = createdBy?.Trim();
        return version;
    }

    /// <summary>
    /// Publishes a specific draft version. Supersedes the previously published version
    /// if any — enforces the "one active published version at a time" invariant.
    /// </summary>
    public void PublishVersion(Guid versionId, string? publishedBy, DateTimeOffset whenUtc)
    {
        if (Status == AcknowledgmentStatus.Archived)
        {
            throw new InvalidOperationException("Cannot publish versions on an archived acknowledgment definition.");
        }

        var target = RequireVersion(versionId);

        var currentlyPublished = _versions.SingleOrDefault(v => v.Status == AcknowledgmentVersionStatus.Published);
        target.MarkPublished(publishedBy, whenUtc);
        currentlyPublished?.MarkSuperseded(target.Id, whenUtc);

        Status = AcknowledgmentStatus.Published;
        CurrentAcknowledgmentVersionId = target.Id;
        UpdatedAtUtc = whenUtc;
        UpdatedBy = publishedBy?.Trim();
    }

    public void ArchiveVersion(Guid versionId, string? archivedBy, DateTimeOffset whenUtc)
    {
        var target = RequireVersion(versionId);
        target.MarkArchived(archivedBy, whenUtc);
        UpdatedAtUtc = whenUtc;
        UpdatedBy = archivedBy?.Trim();
    }

    /// <summary>Archives the definition: master goes Archived, and any currently
    /// published version is retired. Superseded versions remain untouched for audit.</summary>
    public void Archive(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == AcknowledgmentStatus.Archived)
        {
            return;
        }

        var published = _versions.SingleOrDefault(v => v.Status == AcknowledgmentVersionStatus.Published);
        published?.ArchiveAsPartOfDefinitionArchive(archivedBy, whenUtc);

        Status = AcknowledgmentStatus.Archived;
        CurrentAcknowledgmentVersionId = null;
        UpdatedAtUtc = whenUtc;
        UpdatedBy = archivedBy?.Trim();
    }

    private AcknowledgmentVersion RequireVersion(Guid versionId)
    {
        return _versions.SingleOrDefault(v => v.Id == versionId)
            ?? throw new InvalidOperationException(
                $"Acknowledgment version {versionId} is not part of definition {Id}.");
    }
}
