using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Доменный интерфейс репозитория для работы с пользовательскими ролями
    /// </summary>
    public interface IUserRolesRepository
    {
        Task<Result> CreateAsync(object userRole);

        IEnumerable<object> GetAllUserRoles();

        Task<IUserRole?> GetAsync(Expression<Func<IUserRole, bool>> predicate, CancellationToken cancellationToken = default);

        Task<IEnumerable<IUserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<IUserRole>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(object userRole);

        Task<Result> DeleteAsync(object userRole, CancellationToken cancellationToken = default);
        
        Task<Result> DeleteAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

        Task<Result> DeleteUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Создает пользовательскую роль, предварительно удалив существующие роли пользователя
        /// </summary>
        /// <param name="roleId">ID роли</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result> CreateUserIdRoleIdAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default);
    }
}

