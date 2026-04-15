using System.Net;
using System.Text.Json;
using Eap.Api.Conventions;
using Eap.Application.Common.Exceptions;
using ApplicationValidationException = Eap.Application.Common.Exceptions.ValidationException;

namespace Eap.Api.Middleware;

/// <summary>
/// Centralized exception handler. Converts known application-layer exceptions
/// into standardized <see cref="ApiError"/> responses and hides internal details
/// for all other exceptions, as required by the implementation rules.
/// </summary>
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
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, errors) = exception switch
        {
            ApplicationValidationException v =>
                ((int)HttpStatusCode.BadRequest, "Validation failed.", v.Errors),
            NotFoundException =>
                ((int)HttpStatusCode.NotFound, "Resource not found.", (IDictionary<string, string[]>?)null),
            ForbiddenAccessException =>
                ((int)HttpStatusCode.Forbidden, "Access forbidden.", (IDictionary<string, string[]>?)null),
            UnauthorizedAccessException =>
                ((int)HttpStatusCode.Unauthorized, "Authentication required.", (IDictionary<string, string[]>?)null),
            _ =>
                ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.", (IDictionary<string, string[]>?)null)
        };

        if (status >= 500)
        {
            _logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled {Status} for {Path}", status, context.Request.Path);
        }

        var payload = new ApiError
        {
            Status = status,
            Title = title,
            Detail = status >= 500 ? null : exception.Message,
            TraceId = context.TraceIdentifier,
            Errors = errors
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response
            .WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions))
            .ConfigureAwait(false);
    }
}
