using Eap.Domain.Common;

namespace Eap.Domain.Policy;

/// <summary>
/// Master policy record (docs/05-data/conceptual-data-model.md §5.1).
/// A Policy aggregates many <see cref="PolicyVersion"/> snapshots across time.
/// The master row carries the long-lived metadata (code, title, owning
/// department, category); versioned business content — effective date, document,
/// summary — belongs to the version (see BR-011, BR-012, BR-014).
/// </summary>
public sealed class Policy : AuditableEntity
{
    private readonly List<PolicyVersion> _versions = new();

    // EF Core
    private Policy() { }

    public Policy(
        string policyCode,
        string title,
        string ownerDepartment,
        string? category,
        string? description,
        string? createdBy)
    {
        if (string.IsNullOrWhiteSpace(policyCode))
        {
            throw new ArgumentException("Policy code is required.", nameof(policyCode));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Policy title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(ownerDepartment))
        {
            throw new ArgumentException("Owner department is required (BR-014).", nameof(ownerDepartment));
        }

        PolicyCode = policyCode.Trim();
        Title = title.Trim();
        OwnerDepartment = ownerDepartment.Trim();
        Category = category?.Trim();
        Description = description?.Trim();
        Status = PolicyStatus.Draft;
        CreatedBy = createdBy?.Trim();
    }

    public string PolicyCode { get; private set; } = default!;

    public string Title { get; private set; } = default!;

    public string OwnerDepartment { get; private set; } = default!;

    public string? Category { get; private set; }

    public string? Description { get; private set; }

    public PolicyStatus Status { get; private set; }

    public Guid? CurrentPolicyVersionId { get; private set; }

    public IReadOnlyCollection<PolicyVersion> Versions => _versions.AsReadOnly();

    /// <summary>Updates editable metadata on the master record.
    /// Master metadata (title, owner, category, description) is mutable across
    /// the policy lifecycle; version-level business content remains immutable.
    /// </summary>
    public void UpdateMetadata(
        string title,
        string ownerDepartment,
        string? category,
        string? description,
        string? updatedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Policy title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(ownerDepartment))
        {
            throw new ArgumentException("Owner department is required (BR-014).", nameof(ownerDepartment));
        }

        Title = title.Trim();
        OwnerDepartment = ownerDepartment.Trim();
        Category = category?.Trim();
        Description = description?.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy?.Trim();
    }

    /// <summary>
    /// Creates a new draft version. The caller supplies a next version number;
    /// we assume responsibility for enforcing per-policy monotonic numbering
    /// lives in the repository (where the query is efficient).
    /// </summary>
    public PolicyVersion CreateDraftVersion(
        int nextVersionNumber,
        string? versionLabel,
        DateOnly? effectiveDate,
        string? summary,
        string? createdBy)
    {
        if (Status == PolicyStatus.Archived)
        {
            throw new InvalidOperationException(
                "Cannot create a draft on an archived policy. Reactivate the policy first.");
        }

        var version = new PolicyVersion(
            policyId: Id,
            versionNumber: nextVersionNumber,
            versionLabel: versionLabel,
            effectiveDate: effectiveDate,
            summary: summary,
            createdBy: createdBy);

        _versions.Add(version);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = createdBy?.Trim();
        return version;
    }

    /// <summary>
    /// Publishes a specific draft version. Supersedes the previously published
    /// version if any — enforces BR-011 (one active published version at a time).
    /// </summary>
    public void PublishVersion(Guid versionId, string? publishedBy, DateTimeOffset whenUtc)
    {
        if (Status == PolicyStatus.Archived)
        {
            throw new InvalidOperationException("Cannot publish versions on an archived policy.");
        }

        var target = RequireVersion(versionId);

        var currentlyPublished = _versions.SingleOrDefault(v => v.Status == PolicyVersionStatus.Published);
        target.MarkPublished(publishedBy, whenUtc);
        currentlyPublished?.MarkSuperseded(target.Id, whenUtc);

        Status = PolicyStatus.Published;
        CurrentPolicyVersionId = target.Id;
        UpdatedAtUtc = whenUtc;
        UpdatedBy = publishedBy?.Trim();
    }

    /// <summary>Archives a draft/superseded version (BR-012: historical records remain).</summary>
    public void ArchiveVersion(Guid versionId, string? archivedBy, DateTimeOffset whenUtc)
    {
        var target = RequireVersion(versionId);
        target.MarkArchived(archivedBy, whenUtc);
        UpdatedAtUtc = whenUtc;
        UpdatedBy = archivedBy?.Trim();
    }

    /// <summary>Archives the policy as a whole: master goes Archived and any
    /// currently published version is retired. Previously-superseded versions
    /// are left untouched and remain accessible for historical reference.</summary>
    public void Archive(string? archivedBy, DateTimeOffset whenUtc)
    {
        if (Status == PolicyStatus.Archived)
        {
            return;
        }

        var published = _versions.SingleOrDefault(v => v.Status == PolicyVersionStatus.Published);
        published?.ArchiveAsPartOfPolicyArchive(archivedBy, whenUtc);

        Status = PolicyStatus.Archived;
        CurrentPolicyVersionId = null;
        UpdatedAtUtc = whenUtc;
        UpdatedBy = archivedBy?.Trim();
    }

    private PolicyVersion RequireVersion(Guid versionId)
    {
        var version = _versions.SingleOrDefault(v => v.Id == versionId)
            ?? throw new InvalidOperationException($"Policy version {versionId} is not part of policy {Id}.");
        return version;
    }
}
