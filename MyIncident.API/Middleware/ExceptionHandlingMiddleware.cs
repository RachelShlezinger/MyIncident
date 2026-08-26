using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyIncident.API.DTOs;

namespace MyIncident.API.Middleware;

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
        catch (DbUpdateConcurrencyException)
        {
            await WriteErrorResponse(context, StatusCodes.Status409Conflict, "Conflict",
                "The record was modified by another user. Please reload and try again.");
        }
        catch (ArgumentException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "BadRequest", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status404NotFound, "NotFound", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError,
                "InternalServerError", "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string error, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Error = error,
            Message = message,
            StatusCode = statusCode
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
