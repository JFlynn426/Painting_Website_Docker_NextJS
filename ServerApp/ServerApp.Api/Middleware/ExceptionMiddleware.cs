using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerApp.Shared.Exceptions;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.Exceptions;

namespace ServerApp.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        switch (exception)
        {
            case PaintingNotFoundException ex:
                response.StatusCode = StatusCodes.Status404NotFound;
                await response.WriteAsJsonAsync(new { error = "Painting not found", message = ex.Message });
                break;

            case ServerApp.Domain.Exceptions.PaintingCategoryNotFoundException ex:
                response.StatusCode = StatusCodes.Status404NotFound;
                await response.WriteAsJsonAsync(new { error = "Painting category not found", message = ex.Message });
                break;

            case PageContentNotFoundException ex:
                response.StatusCode = StatusCodes.Status404NotFound;
                await response.WriteAsJsonAsync(new { error = "Page content not found", message = ex.Message });
                break;

            case PaintingSlugAlreadyExistsException ex:
                response.StatusCode = StatusCodes.Status409Conflict;
                await response.WriteAsJsonAsync(new { error = "Painting slug already exists", message = ex.Message });
                break;

            case PaintingCategoryNameAlreadyExistsException ex:
                response.StatusCode = StatusCodes.Status409Conflict;
                await response.WriteAsJsonAsync(new { error = "Painting category name already exists", message = ex.Message });
                break;

            case PaintingMustHaveAnAssignedCategoryException ex:
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsJsonAsync(new { error = "Invalid category", message = ex.Message });
                break;

            case StringValueObjectException ex:
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsJsonAsync(new { error = "Invalid input", message = ex.Message });
                break;

            case DecimalValueObjectException ex:
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsJsonAsync(new { error = "Invalid input", message = ex.Message });
                break;

            case ServerAppException ex:
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsJsonAsync(new { error = "Server error", message = ex.Message });
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                await response.WriteAsJsonAsync(new { error = "Internal server error", message = "An unexpected error occurred" });
                break;
        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}