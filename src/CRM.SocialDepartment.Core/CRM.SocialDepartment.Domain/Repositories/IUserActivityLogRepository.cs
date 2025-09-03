using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Common;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с логами активности пользователей
    /// </summary>
    public interface IUserActivityLogRepository
    {
        /// <summary>
        /// Добавить лог активности
        /// </summary>
        Task<Result> AddAsync(UserActivityLog log);

        /// <summary>
        /// Получить все логи активности
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetAllAsync();

        /// <summary>
        /// Получить логи активности пользователя
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Получить логи активности по типу действия
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetByActivityTypeAsync(UserActivityType activityType);

        /// <summary>
        /// Получить логи активности по диапазону дат
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Получить логи активности по сущности
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetByEntityAsync(string entityType, Guid entityId);

        /// <summary>
        /// Получить логи активности с фильтрацией
        /// </summary>
        Task<IEnumerable<UserActivityLog>> GetFilteredAsync(
            Guid? userId = null,
            UserActivityType? activityType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? entityType = null,
            int? page = null,
            int? pageSize = null);

        /// <summary>
        /// Получить количество логов с фильтрацией
        /// </summary>
        Task<int> GetCountAsync(
            Guid? userId = null,
            UserActivityType? activityType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? entityType = null);

        /// <summary>
        /// Удалить старые логи (старше указанной даты)
        /// </summary>
        Task<Result> DeleteOldLogsAsync(DateTime olderThan);
    }
}
