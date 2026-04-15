using Eap.Application.Common.Exceptions;
using Eap.Application.Policies.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Infrastructure.Policies.Documents;

/// <summary>
/// MVP file-system storage for policy documents.
/// <para>
/// Layout: <c>{RootPath}/{policyId}/{versionId}/{storageFileName}</c>.
/// The <c>storageReference</c> returned to the caller is the path relative
/// to <c>RootPath</c> so the storage root can move without rewriting DB rows.
/// </para>
/// </summary>
internal sealed class FileSystemPolicyDocumentStorage : IPolicyDocumentStorage
{
    private readonly PolicyDocumentStorageOptions _options;
    private readonly ILogger<FileSystemPolicyDocumentStorage> _logger;

    public FileSystemPolicyDocumentStorage(
        IOptions<PolicyDocumentStorageOptions> options,
        ILogger<FileSystemPolicyDocumentStorage> logger)
    {
        _options = options.Value;
        _logger = logger;

        Directory.CreateDirectory(_options.RootPath);
    }

    public async Task<PolicyDocumentStorageResult> StoreAsync(
        Guid policyId,
        Guid policyVersionId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);
        ValidateAllowed(fileName, contentType);

        var extension = Path.GetExtension(fileName);
        var storageFileName = $"{Guid.NewGuid():N}{extension}";

        var relativePath = Path.Combine(policyId.ToString("N"), policyVersionId.ToString("N"), storageFileName);
        var absolutePath = Path.Combine(_options.RootPath, relativePath);

        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        long bytesWritten;
        await using (var fileStream = new FileStream(
                         absolutePath,
                         FileMode.CreateNew,
                         FileAccess.Write,
                         FileShare.None,
                         bufferSize: 81920,
                         useAsync: true))
        {
            await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            bytesWritten = fileStream.Length;
        }

        if (bytesWritten > _options.MaxFileSizeBytes)
        {
            // Remove the oversized file to keep the store tidy, then fail.
            TryDelete(absolutePath);
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(fileName),
                    $"File exceeds configured maximum size of {_options.MaxFileSizeBytes} bytes."),
            });
        }

        _logger.LogInformation(
            "Stored policy document {FileName} ({Size} bytes) at {StorageReference}.",
            fileName, bytesWritten, relativePath);

        return new PolicyDocumentStorageResult(relativePath, bytesWritten);
    }

    public Task<Stream> OpenReadAsync(string storageReference, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageReference))
        {
            throw new ArgumentException("Storage reference is required.", nameof(storageReference));
        }

        var absolutePath = ResolveAbsolutePath(storageReference);

        if (!File.Exists(absolutePath))
        {
            throw new NotFoundException($"The document at '{storageReference}' is missing on disk.");
        }

        Stream stream = new FileStream(
            absolutePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageReference, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageReference))
        {
            return Task.CompletedTask;
        }

        var absolutePath = ResolveAbsolutePath(storageReference);
        TryDelete(absolutePath);
        return Task.CompletedTask;
    }

    private void ValidateAllowed(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
        var contentTypeNormalized = contentType?.Trim().ToLowerInvariant() ?? string.Empty;

        var failures = new List<FluentValidation.Results.ValidationFailure>();

        if (!_options.AllowedExtensions.Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add(new FluentValidation.Results.ValidationFailure(
                nameof(fileName),
                $"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", _options.AllowedExtensions)}."));
        }

        if (!_options.AllowedContentTypes.Any(ct => string.Equals(ct, contentTypeNormalized, StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add(new FluentValidation.Results.ValidationFailure(
                nameof(contentType),
                $"Content type '{contentType}' is not allowed."));
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }

    private string ResolveAbsolutePath(string storageReference)
    {
        // Defence-in-depth: make sure callers can't escape the configured root
        // with "..\..\etc" in a stored reference.
        var root = Path.GetFullPath(_options.RootPath);
        var combined = Path.GetFullPath(Path.Combine(root, storageReference));

        if (!combined.StartsWith(root, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Resolved document path is outside the configured storage root.");
        }

        return combined;
    }

    private void TryDelete(string absolutePath)
    {
        try
        {
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Failed to delete document at {Path}.", absolutePath);
        }
    }
}
