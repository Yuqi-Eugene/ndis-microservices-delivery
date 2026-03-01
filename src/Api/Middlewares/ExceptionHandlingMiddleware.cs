using System.Net;
using System.Security.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteProblem(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (AuthenticationException ex)
        {
            await WriteProblem(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteValidationProblem(context, ex);
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
