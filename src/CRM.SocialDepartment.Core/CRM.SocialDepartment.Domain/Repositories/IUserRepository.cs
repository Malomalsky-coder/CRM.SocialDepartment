using CRM.SocialDepartment.Domain.Common;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Доменный интерфейс репозитория для работы с пользователями
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат операции</returns>
        Task<Result> CreateAsync(object user, string password);

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns>Список всех пользователей</returns>
        IEnumerable<object> GetAllUsers();
    }
} 