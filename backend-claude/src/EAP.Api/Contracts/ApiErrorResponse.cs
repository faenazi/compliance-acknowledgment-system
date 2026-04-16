namespace EAP.Api.Contracts;

public sealed class ApiErrorResponse
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<ApiFieldError>? Errors { get; init; }
    public string? TraceId { get; init; }
}

public sealed class ApiFieldError
{
    public string Field { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
