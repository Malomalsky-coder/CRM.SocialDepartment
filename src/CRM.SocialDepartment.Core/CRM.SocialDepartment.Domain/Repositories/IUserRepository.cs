using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using System.Linq.Expressions;

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

        IEnumerable<object> GetAllUsers();

        Task<Result> VerifyPasswordAsync(string userName, string password);

        Task<IUser?> GetAsync(Expression<Func<IUser, bool>> predicate, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(object user, string? password = null);

        Task<Result> DeleteAsync(object user, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Удалить пользователя по имени пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(string userName, CancellationToken cancellationToken = default);
    }
} 