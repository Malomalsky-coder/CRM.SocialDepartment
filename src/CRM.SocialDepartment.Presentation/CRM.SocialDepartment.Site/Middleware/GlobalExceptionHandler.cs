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
                InvalidDocumentNumberException invalidDocEx => (HttpStatusCode.BadRequest, FormatDocumentError(invalidDocEx.Message), "error"),
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

        /// <summary>
        /// Форматирует техническое сообщение об ошибке документа в понятное пользователю
        /// </summary>
        private static string FormatDocumentError(string technicalMessage)
        {
            if (technicalMessage.Contains("PassportDocument"))
            {
                return "Неверный формат паспорта. Укажите серию и номер в формате: 1234 567890";
            }
            else if (technicalMessage.Contains("MedicalPolicyDocument"))
            {
                return "Неверный формат полиса ОМС. Номер должен содержать 16 цифр";
            }
            else if (technicalMessage.Contains("SnilsDocument"))
            {
                return "Неверный формат СНИЛС. Укажите номер в формате: 123-456-789 01";
            }
            
            // Если тип документа не определен, возвращаем оригинальное сообщение
            return technicalMessage;
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