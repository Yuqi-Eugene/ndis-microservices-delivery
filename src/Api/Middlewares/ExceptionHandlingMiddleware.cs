using System.Net;
using System.Security.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Middlewares;

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
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogInformation(ex, "Resource not found for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Forbidden request for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning(ex, "Authentication failed for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteValidationProblem(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblem(HttpContext context, HttpStatusCode code, string? detail)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/problem+json";

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
