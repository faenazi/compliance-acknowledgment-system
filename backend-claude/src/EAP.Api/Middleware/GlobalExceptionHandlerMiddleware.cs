using System.Diagnostics;
using System.Text.Json;
using EAP.Api.Contracts;
using FluentValidation;

namespace EAP.Api.Middleware;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedException(context, ex);
        }
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors
            .Select(e => new ApiFieldError
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            })
            .ToList();

        var response = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "One or more validation errors occurred.",
            Errors = errors,
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private async Task HandleUnexpectedException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "An unexpected error occurred.",
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
