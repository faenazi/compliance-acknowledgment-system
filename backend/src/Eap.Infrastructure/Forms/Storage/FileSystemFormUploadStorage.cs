using Eap.Application.Common.Exceptions;
using Eap.Application.Forms.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Forms.Storage;

/// <summary>
/// File-system backed storage for form-upload fields (BR-076).
/// Layout: <c>{RootPath}/{submissionId}/{fieldKey}/{storageFileName}</c>.
/// </summary>
internal sealed class FileSystemFormUploadStorage : IFormUploadStorage
{
    private readonly FormUploadStorageOptions _options;
    private readonly ILogger<FileSystemFormUploadStorage> _logger;

    public FileSystemFormUploadStorage(
        IOptions<FormUploadStorageOptions> options,
        ILogger<FileSystemFormUploadStorage> logger)
    {
        _options = options.Value;
        _logger = logger;
        Directory.CreateDirectory(_options.RootPath);
    }

    public async Task<FormUploadStorageResult> StoreAsync(
        Guid submissionId,
        string fieldKey,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);
        ValidateAllowed(fileName, contentType);

        var extension = Path.GetExtension(fileName);
        var storageFileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(submissionId.ToString("N"), fieldKey, storageFileName);
        var absolutePath = Path.Combine(_options.RootPath, relativePath);

        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        long bytesWritten;
        await using (var fileStream = new FileStream(
                         absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                         bufferSize: 81920, useAsync: true))
        {
            await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            bytesWritten = fileStream.Length;
        }

        if (bytesWritten > _options.MaxFileSizeBytes)
        {
            TryDelete(absolutePath);
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(fileName),
                    $"File exceeds maximum size of {_options.MaxFileSizeBytes} bytes."),
            });
        }

        _logger.LogInformation(
            "Stored form upload {FileName} ({Size} bytes) at {StorageReference}.",
            fileName, bytesWritten, relativePath);

        return new FormUploadStorageResult(relativePath, bytesWritten);
    }

    public Task<Stream> OpenReadAsync(string storageReference, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageReference))
            throw new ArgumentException("Storage reference is required.", nameof(storageReference));

        var absolutePath = ResolveAbsolutePath(storageReference);
        if (!File.Exists(absolutePath))
            throw new NotFoundException($"The uploaded file at '{storageReference}' is missing on disk.");

        Stream stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 81920, useAsync: true);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageReference, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageReference))
            return Task.CompletedTask;

        var absolutePath = ResolveAbsolutePath(storageReference);
        TryDelete(absolutePath);
        return Task.CompletedTask;
    }

    private void ValidateAllowed(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
        var ct = contentType?.Trim().ToLowerInvariant() ?? string.Empty;
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        if (!_options.AllowedExtensions.Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase)))
            failures.Add(new FluentValidation.Results.ValidationFailure(
                nameof(fileName),
                $"Extension '{extension}' is not allowed. Allowed: {string.Join(", ", _options.AllowedExtensions)}."));

        if (!_options.AllowedContentTypes.Any(c => string.Equals(c, ct, StringComparison.OrdinalIgnoreCase)))
            failures.Add(new FluentValidation.Results.ValidationFailure(
                nameof(contentType),
                $"Content type '{contentType}' is not allowed."));

        if (failures.Count > 0)
            throw new ValidationException(failures);
    }

    private string ResolveAbsolutePath(string storageReference)
    {
        var root = Path.GetFullPath(_options.RootPath);
        var combined = Path.GetFullPath(Path.Combine(root, storageReference));
        if (!combined.StartsWith(root, StringComparison.Ordinal))
            throw new InvalidOperationException("Resolved path is outside the configured storage root.");
        return combined;
    }

    private void TryDelete(string absolutePath)
    {
        try { if (File.Exists(absolutePath)) File.Delete(absolutePath); }
        catch (IOException ex) { _logger.LogWarning(ex, "Failed to delete file at {Path}.", absolutePath); }
    }
}
