using System.Text.Json;

namespace CRM.SocialDepartment.Domain.Entities
{
    /// <summary>
    /// Типы действий пользователей
    /// </summary>
    public enum UserActivityType
    {
        /// <summary>
        /// Авторизация в системе
        /// </summary>
        Login,
        
        /// <summary>
        /// Выход из системы
        /// </summary>
        Logout,
        
        /// <summary>
        /// Запрос на получение данных
        /// </summary>
        DataRequest,
        
        /// <summary>
        /// Создание сущности
        /// </summary>
        Create,
        
        /// <summary>
        /// Редактирование сущности
        /// </summary>
        Update,
        
        /// <summary>
        /// Удаление сущности
        /// </summary>
        Delete
    }

    /// <summary>
    /// Лог активности пользователя
    /// </summary>
    public class UserActivityLog
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// ID пользователя
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Тип действия
        /// </summary>
        public UserActivityType ActivityType { get; set; }
        
        /// <summary>
        /// Название действия
        /// </summary>
        public string ActionName { get; set; } = string.Empty;
        
        /// <summary>
        /// Описание действия
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Тип сущности (если применимо)
        /// </summary>
        public string? EntityType { get; set; }
        
        /// <summary>
        /// ID сущности (если применимо)
        /// </summary>
        public Guid? EntityId { get; set; }
        
        /// <summary>
        /// Данные до изменения (для Update)
        /// </summary>
        public string? BeforeChange { get; set; }
        
        /// <summary>
        /// Данные после изменения (для Update)
        /// </summary>
        public string? AfterChange { get; set; }
        
        /// <summary>
        /// IP адрес пользователя
        /// </summary>
        public string? IpAddress { get; set; }
        
        /// <summary>
        /// User Agent браузера
        /// </summary>
        public string? UserAgent { get; set; }
        
        /// <summary>
        /// URL запроса
        /// </summary>
        public string? RequestUrl { get; set; }
        
        /// <summary>
        /// HTTP метод
        /// </summary>
        public string? HttpMethod { get; set; }
        
        /// <summary>
        /// Время выполнения действия
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Дополнительные данные в JSON формате
        /// </summary>
        public string? AdditionalData { get; set; }

        public UserActivityLog()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Создать лог для авторизации
        /// </summary>
        public static UserActivityLog CreateLoginLog(Guid userId, string userName, string fullName, string? ipAddress = null, string? userAgent = null)
        {
            return new UserActivityLog
            {
                UserId = userId,
                UserName = userName,
                FullName = fullName,
                ActivityType = UserActivityType.Login,
                ActionName = "Авторизация",
                Description = $"Пользователь {fullName} авторизовался в системе",
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Создать лог для запроса данных
        /// </summary>
        public static UserActivityLog CreateDataRequestLog(Guid userId, string userName, string fullName, string actionName, string description, string? requestUrl = null, string? httpMethod = null, string? ipAddress = null)
        {
            return new UserActivityLog
            {
                UserId = userId,
                UserName = userName,
                FullName = fullName,
                ActivityType = UserActivityType.DataRequest,
                ActionName = actionName,
                Description = description,
                RequestUrl = requestUrl,
                HttpMethod = httpMethod,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Создать лог для создания сущности
        /// </summary>
        public static UserActivityLog CreateEntityLog(Guid userId, string userName, string fullName, UserActivityType activityType, string entityType, Guid entityId, string actionName, string description, object? entityData = null, string? ipAddress = null)
        {
            return new UserActivityLog
            {
                UserId = userId,
                UserName = userName,
                FullName = fullName,
                ActivityType = activityType,
                ActionName = actionName,
                Description = description,
                EntityType = entityType,
                EntityId = entityId,
                AfterChange = entityData != null ? JsonSerializer.Serialize(entityData, new JsonSerializerOptions { WriteIndented = true }) : null,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Создать лог для обновления сущности
        /// </summary>
        public static UserActivityLog CreateUpdateLog(Guid userId, string userName, string fullName, string entityType, Guid entityId, string actionName, string description, object? beforeData = null, object? afterData = null, string? ipAddress = null)
        {
            return new UserActivityLog
            {
                UserId = userId,
                UserName = userName,
                FullName = fullName,
                ActivityType = UserActivityType.Update,
                ActionName = actionName,
                Description = description,
                EntityType = entityType,
                EntityId = entityId,
                BeforeChange = beforeData != null ? JsonSerializer.Serialize(beforeData, new JsonSerializerOptions { WriteIndented = true }) : null,
                AfterChange = afterData != null ? JsonSerializer.Serialize(afterData, new JsonSerializerOptions { WriteIndented = true }) : null,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
