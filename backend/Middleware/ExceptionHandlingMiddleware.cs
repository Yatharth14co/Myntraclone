using System.Net;
using FluentValidation;
using ECommerceApi.DTOs;
using ECommerceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        ApiResponse response;

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiResponse
                {
                    Success = false,
                    Message = validationEx.Message,
                    Errors = validationEx.Errors
                };
                break;

            case BusinessException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiResponse
                {
                    Success = false,
                    Message = businessEx.Message,
                    Errors = businessEx.Errors
                };
                break;

            case ResourceNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ApiResponse
                {
                    Success = false,
                    Message = exception.Message
                };
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiResponse
                {
                    Success = false,
                    Message = exception.Message
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ApiResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Extension methods for middleware registration
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
