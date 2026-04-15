namespace Eap.Api.Conventions;

/// <summary>
/// Standardized error body returned by the EAP API.
/// Every non-2xx response produced by the global exception middleware uses this shape.
/// </summary>
public sealed class ApiError
{
    public int Status { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Detail { get; init; }

    public string? TraceId { get; init; }

    public IDictionary<string, string[]>? Errors { get; init; }
}
