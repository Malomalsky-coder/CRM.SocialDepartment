using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Site.Models;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Text.Json;

namespace CRM.SocialDepartment.Site.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message, messageType) = exception switch
            {
                DomainException domainEx => (HttpStatusCode.BadRequest, domainEx.Message, "error"),
                ArgumentNullException argNullEx => (HttpStatusCode.BadRequest, argNullEx.Message, "error"),
                KeyNotFoundException keyNotFoundEx => (HttpStatusCode.NotFound, keyNotFoundEx.Message, "error"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "У вас нет прав для выполнения этой операции", "error"),
                _ => (HttpStatusCode.InternalServerError, "Произошла внутренняя ошибка сервера", "error")
            };

            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            response.StatusCode = (int)statusCode;

            var apiResponse = ApiResponse<object>.Error(message, messageType);
            var json = JsonSerializer.Serialize(apiResponse);

            await response.WriteAsync(json);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>();
        }
    }
} 