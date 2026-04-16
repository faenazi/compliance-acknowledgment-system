using System.Text.Json;
using Eap.Api.Contracts.Error;
using Eap.Application.Common.Exceptions;

namespace Eap.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Request validation failed.");
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "validation_error", ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "server_error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string type,
        string title,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ApiErrorResponse
        {
            Type = type,
            Title = title,
            Status = statusCode,
            TraceId = context.TraceIdentifier,
            Errors = errors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, response, SerializerOptions);
    }
}
