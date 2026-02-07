using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Exceptions;

namespace SecureBank.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleAsync(context, ex);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode;
        string title;
        string detail;

        if (ex is ConflictException)
        {
            statusCode = HttpStatusCode.Conflict;
            title = "Conflict";
            detail = ex.Message;
        }
        else if (ex is ForbiddenException)
        {
            statusCode = HttpStatusCode.Forbidden;
            title = "Forbidden";
            detail = ex.Message;
        }
        else if (ex is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            title = "Unauthorized";
            detail = ex.Message;
        }
        else if (ex is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            title = "Not Found";
            detail = ex.Message;
        }
        else if (ex is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest;
            title = "Bad Request";
            detail = ex.Message;
        }
        else if (ex is InvalidOperationException && ex.Message.Contains("pending"))
        {
            statusCode = HttpStatusCode.Conflict;
            title = "Conflict";
            detail = ex.Message;
        }
        else if (ex is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            title = "Bad Request";
            detail = ex.Message;
        }
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            title = "Internal Server Error";
            detail = "An error occurred.";
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Title = title,
            Status = (int)statusCode,
            Detail = detail
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
