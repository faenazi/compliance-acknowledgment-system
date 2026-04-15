using Eap.Domain.Common;

namespace Eap.Domain.Policy;

/// <summary>
/// Immutable-once-published business snapshot of a <see cref="Policy"/>
/// (docs/05-data/conceptual-data-model.md §5.2,
/// docs/03-functional-requirements/lifecycle-models.md §3.2).
/// </summary>
public sealed class PolicyVersion : AuditableEntity
{
    private PolicyDocument? _document;

    // EF Core
    private PolicyVersion() { }

    internal PolicyVersion(
        Guid policyId,
        int versionNumber,
        string? versionLabel,
        DateOnly? effectiveDate,
        string? summary,
        string? createdBy)
    {
        if (policyId == Guid.Empty)
        {
            throw new ArgumentException("Policy id is required.", nameof(policyId));
        }

        if (versionNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(versionNumber), "Version number must be positive.");
        }

        PolicyId = policyId;
        VersionNumber = versionNumber;
        VersionLabel = versionLabel?.Trim();
        EffectiveDate = effectiveDate;
        Summary = summary?.Trim();
        Status = PolicyVersionStatus.Draft;
        CreatedBy = createdBy?.Trim();
    }

    public Guid PolicyId { get; private set; }

    public int VersionNumber { get; private set; }

    public string? VersionLabel { get; private set; }

    public DateOnly? EffectiveDate { get; private set; }

    public string? Summary { get; private set; }

    public PolicyVersionStatus Status { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public string? PublishedBy { get; private set; }

    public DateTimeOffset? ArchivedAtUtc { get; private set; }

    public string? ArchivedBy { get; private set; }

    public Guid? SupersededByPolicyVersionId { get; private set; }

    public PolicyDocument? Document => _document;

    /// <summary>True when a document has been attached (BR-010 prerequisite).</summary>
    public bool HasDocument => _document is not null;

    /// <summary>Edits to metadata are only allowed while the version is still a draft (BR-003).</summary>
    public void UpdateDraftMetadata(
        string? versionLabel,
        DateOnly? effectiveDate,
        string? summary,
        string? updatedBy)
    {
        EnsureDraft("update");
        VersionLabel = versionLabel?.Trim();
        EffectiveDate = effectiveDate;
        Summary = summary?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }

    /// <summary>
    /// Attaches (or replaces) the draft's uploaded document. Only allowed while the
    /// version is in Draft — published versions are immutable (BR-003).
    /// </summary>
    public void AttachDocument(PolicyDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        EnsureDraft("attach a document to");

        if (document.PolicyVersionId != Id)
        {
            throw new InvalidOperationException("Document does not belong to this policy version.");
        }

        _document = document;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>Publishes the version. Callers are responsible for enforcing the
    /// "one published version per policy" uniqueness rule (BR-011) at the aggregate
    /// level via <see cref="Policy.PublishVersion"/>.</summary>
    internal void MarkPublished(string? publishedBy, DateTimeOffset whenUtc)
    {
        EnsureDraft("publish");

        if (!HasDocument)
        {
            throw new InvalidOperationException("A policy version cannot be published without a document (BR-010).");
        }

        Status = PolicyVersionStatus.Published;
        PublishedAtUtc = whenUtc;
        PublishedBy = publishedBy?.Trim();
        UpdatedAtUtc = whenUtc;
        UpdatedBy = PublishedBy;
    }

    internal void MarkSuperseded(Guid newVersionId, DateTimeOffset whenUtc)
    {
        if (Status != PolicyVersionStatus.Published)
        {
            throw new InvalidOperationException("Only a published version can be superseded.");
        }

        if (newVersionId == Guid.Empty || newVersionId == Id)
        {
            throw new ArgumentException("A valid successor version id is required.", nameof(newVersionId));
        }

        Status = PolicyVersionStatus.Superseded;
        SupersededByPolicyVersionId = newVersionId;
        UpdatedAtUtc = whenUtc;
    }

    internal void MarkArchived(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == PolicyVersionStatus.Archived)
        {
            return;
        }

        if (Status == PolicyVersionStatus.Published)
        {
            throw new InvalidOperationException(
                "Archive the policy or publish a successor first; a currently published version cannot be archived directly.");
        }

        ApplyArchive(archivedBy, whenUtc);
    }

    /// <summary>
    /// Archive path used when the policy itself is being archived. Unlike
    /// <see cref="MarkArchived"/> this intentionally allows archiving a
    /// currently-published version, because the policy is going dark.
    /// </summary>
    internal void ArchiveAsPartOfPolicyArchive(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == PolicyVersionStatus.Archived)
        {
            return;
        }

        ApplyArchive(archivedBy, whenUtc);
    }

    private void ApplyArchive(string? archivedBy, DateTimeOffset whenUtc)
    {
        Status = PolicyVersionStatus.Archived;
        ArchivedAtUtc = whenUtc;
        ArchivedBy = archivedBy?.Trim();
        UpdatedAtUtc = whenUtc;
    }

    private void EnsureDraft(string attemptedOperation)
    {
        if (Status != PolicyVersionStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Cannot {attemptedOperation} policy version {Id} because it is not a draft (BR-003).");
        }
    }
}
