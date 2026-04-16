using System.ComponentModel.DataAnnotations;

namespace Eap.Infrastructure.Forms.Storage;

/// <summary>
/// Configuration for file uploads attached to form-based disclosure submissions
/// (BR-076). Values are bound from the <c>FormUploads</c> section.
/// </summary>
public sealed class FormUploadStorageOptions
{
    public const string SectionName = "FormUploads";

    [Required(AllowEmptyStrings = false)]
    public string RootPath { get; set; } = default!;

    [Range(1, long.MaxValue)]
    public long MaxFileSizeBytes { get; set; } = 10L * 1024 * 1024; // 10 MB default

    [MinLength(1)]
    public string[] AllowedExtensions { get; set; } = new[]
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx",
    };

    [MinLength(1)]
    public string[] AllowedContentTypes { get; set; } = new[]
    {
        "application/pdf",
        "image/png",
        "image/jpeg",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    };
}
