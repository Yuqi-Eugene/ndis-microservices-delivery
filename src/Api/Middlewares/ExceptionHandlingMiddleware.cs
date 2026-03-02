using System.Net;
using System.Security.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Middlewares;

// This middleware centralizes exception-to-HTTP translation.
// Controllers and handlers can throw meaningful exceptions, and this layer converts them into API-safe responses.
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // Continue to the next middleware/controller in the pipeline.
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            // Missing resources become 404 responses.
            _logger.LogInformation(ex, "Resource not found for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            // This project uses UnauthorizedAccessException to mean "you are authenticated, but not allowed".
            _logger.LogWarning(ex, "Forbidden request for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (AuthenticationException ex)
        {
            // Authentication failures become 401 responses.
            _logger.LogWarning(ex, "Authentication failed for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // InvalidOperationException is used here for domain/business rule violations.
            _logger.LogWarning(ex, "Invalid operation for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ValidationException ex)
        {
            // FluentValidation failures become structured validation payloads.
            _logger.LogWarning(ex, "Validation failed for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteValidationProblem(context, ex);
        }
        catch (Exception ex)
        {
            // Any unknown exception is treated as a server error without leaking internal details.
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblem(HttpContext context, HttpStatusCode code, string? detail)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/problem+json";

        // ProblemDetails is the standard ASP.NET Core format for machine-readable API errors.
        var problem = new ProblemDetails
        {
            Status = (int)code,
            Title = code.ToString(),
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteValidationProblem(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/problem+json";

        // Group validation failures by property name so clients can display field-specific errors cleanly.
        var problem = new ValidationProblemDetails(
            ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).Distinct().ToArray()))
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred."
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
