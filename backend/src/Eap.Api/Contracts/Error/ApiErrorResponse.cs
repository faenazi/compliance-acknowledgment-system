namespace Eap.Api.Contracts.Error;

public sealed class ApiErrorResponse
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string TraceId { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }
}
