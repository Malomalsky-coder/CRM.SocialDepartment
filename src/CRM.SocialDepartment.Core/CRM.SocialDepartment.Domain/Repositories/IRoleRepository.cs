using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Доменный интерфейс репозитория для работы с ролями
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Создать новую роль
        /// </summary>
        /// <param name="role">Роль</param>
        /// <returns>Результат операции</returns>
        Task<Result> CreateAsync(object role);

        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <returns>Коллекция ролей</returns>
        IEnumerable<object> GetAllRoles();

        /// <summary>
        /// Получить роль по предикату
        /// </summary>
        /// <param name="predicate">Предикат для поиска</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Роль или null</returns>
        Task<IRole?> GetAsync(Expression<Func<IRole, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить роль
        /// </summary>
        /// <param name="role">Роль</param>
        /// <returns>Результат операции</returns>
        Task<Result> UpdateAsync(object role);

        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <param name="role">Роль</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(object role, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Удалить роль по имени
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(string roleName, CancellationToken cancellationToken = default);
    }
}




