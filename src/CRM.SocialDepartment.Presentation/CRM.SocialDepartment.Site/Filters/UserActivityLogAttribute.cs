using CRM.SocialDepartment.Application.UserActivityLogs;
using CRM.SocialDepartment.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CRM.SocialDepartment.Site.Filters
{
    /// <summary>
    /// Атрибут для автоматического логирования действий пользователей
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserActivityLogAttribute : Attribute, IAsyncActionFilter
    {
        private readonly UserActivityType _activityType;
        private readonly string _actionName;
        private readonly string _description;
        private readonly string? _entityType;
        private readonly bool _logRequestData;
        private readonly bool _logResponseData;

        public UserActivityLogAttribute(
            UserActivityType activityType,
            string actionName,
            string description,
            string? entityType = null,
            bool logRequestData = false,
            bool logResponseData = false)
        {
            _activityType = activityType;
            _actionName = actionName;
            _description = description;
            _entityType = entityType;
            _logRequestData = logRequestData;
            _logResponseData = logResponseData;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            // Проверяем, что пользователь авторизован
            if (!user.Identity?.IsAuthenticated == true)
            {
                await next();
                return;
            }

            // Получаем информацию о пользователе
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = user.FindFirst(ClaimTypes.Name);
            var firstNameClaim = user.FindFirst("FirstName");
            var lastNameClaim = user.FindFirst("LastName");

            if (userIdClaim == null || userNameClaim == null)
            {
                await next();
                return;
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await next();
                return;
            }

            var userName = userNameClaim.Value;
            var firstName = firstNameClaim?.Value ?? "";
            var lastName = lastNameClaim?.Value ?? "";
            var fullName = $"{firstName} {lastName}".Trim();

            // Получаем данные запроса (если нужно)
            object? requestData = null;
            if (_logRequestData)
            {
                requestData = ExtractRequestData(context);
            }

            // Выполняем действие
            var resultContext = await next();

            // Получаем данные ответа (если нужно)
            object? responseData = null;
            if (_logResponseData && resultContext.Result is ObjectResult objectResult)
            {
                responseData = objectResult.Value;
            }

            // Логируем действие
            try
            {
                var loggerService = httpContext.RequestServices.GetRequiredService<UserActivityLoggerService>();

                // Определяем ID сущности из параметров запроса или результата
                Guid? entityId = ExtractEntityId(context, resultContext);

                await loggerService.LogActionAsync(
                    _activityType,
                    _actionName,
                    _description,
                    _entityType,
                    entityId,
                    requestData,
                    responseData);
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                var logger = httpContext.RequestServices.GetRequiredService<ILogger<UserActivityLogAttribute>>();
                logger.LogError(ex, "Ошибка при логировании активности пользователя");
            }
        }

        private object? ExtractRequestData(ActionExecutingContext context)
        {
            // Извлекаем данные из параметров действия
            var parameters = context.ActionArguments;
            if (parameters.Count == 0)
                return null;

            // Возвращаем первый параметр или все параметры
            return parameters.Count == 1 ? parameters.First().Value : parameters;
        }

        private Guid? ExtractEntityId(ActionExecutingContext context, ActionExecutedContext resultContext)
        {
            // Пытаемся извлечь ID из параметров запроса
            foreach (var parameter in context.ActionArguments.Values)
            {
                if (parameter == null) continue;

                // Проверяем, есть ли свойство Id
                var idProperty = parameter.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(parameter);
                    if (idValue is Guid guid)
                        return guid;
                }

                // Проверяем, является ли параметр сам по себе Guid
                if (parameter is Guid guidParam)
                    return guidParam;
            }

            // Пытаемся извлечь ID из результата
            if (resultContext.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var idProperty = objectResult.Value.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(objectResult.Value);
                    if (idValue is Guid guid)
                        return guid;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Атрибут для логирования авторизации
    /// </summary>
    public class LogLoginAttribute : UserActivityLogAttribute
    {
        public LogLoginAttribute() : base(UserActivityType.Login, "Авторизация", "Пользователь авторизовался в системе")
        {
        }
    }

    /// <summary>
    /// Атрибут для логирования запросов данных
    /// </summary>
    public class LogDataRequestAttribute : UserActivityLogAttribute
    {
        public LogDataRequestAttribute(string actionName, string description) 
            : base(UserActivityType.DataRequest, actionName, description)
        {
        }
    }

    /// <summary>
    /// Атрибут для логирования создания сущностей
    /// </summary>
    public class LogCreateAttribute : UserActivityLogAttribute
    {
        public LogCreateAttribute(string entityType, string actionName, string description, bool logRequestData = true) 
            : base(UserActivityType.Create, actionName, description, entityType, logRequestData, true)
        {
        }
    }

    /// <summary>
    /// Атрибут для логирования обновления сущностей
    /// </summary>
    public class LogUpdateAttribute : UserActivityLogAttribute
    {
        public LogUpdateAttribute(string entityType, string actionName, string description, bool logRequestData = true) 
            : base(UserActivityType.Update, actionName, description, entityType, logRequestData, true)
        {
        }
    }

    /// <summary>
    /// Атрибут для логирования удаления сущностей
    /// </summary>
    public class LogDeleteAttribute : UserActivityLogAttribute
    {
        public LogDeleteAttribute(string entityType, string actionName, string description) 
            : base(UserActivityType.Delete, actionName, description, entityType, false, false)
        {
        }
    }
}
