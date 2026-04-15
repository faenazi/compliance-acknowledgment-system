using System.ComponentModel.DataAnnotations;

namespace Eap.Infrastructure.Policies.Documents;

/// <summary>
/// Configuration for the file-system backed implementation of
/// <see cref="Eap.Application.Policies.Abstractions.IPolicyDocumentStorage"/>.
/// Values are bound from the <c>PolicyDocuments</c> section and validated at
/// start-up (missing config is a fail-fast error, not a runtime surprise).
/// </summary>
public sealed class PolicyDocumentStorageOptions
{
    public const string SectionName = "PolicyDocuments";

    /// <summary>
    /// Absolute or relative path to the root directory under which uploaded
    /// documents are persisted. Documents are namespaced by policy id and
    /// version id so multiple deployments can share a storage root if needed.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string RootPath { get; set; } = default!;

    /// <summary>Maximum accepted upload size, in bytes. Requests over the limit are rejected (413).</summary>
    [Range(1, long.MaxValue)]
    public long MaxFileSizeBytes { get; set; } = 25L * 1024 * 1024; // 25 MB default

    /// <summary>Whitelisted extensions (lower-case, leading dot). MVP uses PDF only.</summary>
    [MinLength(1)]
    public string[] AllowedExtensions { get; set; } = new[] { ".pdf" };

    /// <summary>Whitelisted MIME types. Kept in sync with <see cref="AllowedExtensions"/>.</summary>
    [MinLength(1)]
    public string[] AllowedContentTypes { get; set; } = new[] { "application/pdf" };
}
