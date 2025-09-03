using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория пользовательских ролей
    /// </summary>
    public class MongoUserRolesRepository : IUserRolesRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle? _session;
        private readonly IMongoCollection<ApplicationUserRole> _userRoles;

        public MongoUserRolesRepository(IMongoDatabase database, IClientSessionHandle? session = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _session = session;
            _userRoles = _database.GetCollection<ApplicationUserRole>("userRoles");
        }

        public async Task<Result> CreateAsync(object userRole)
        {
            try
            {
                if (userRole is not ApplicationUserRole applicationUserRole)
                {
                    // Создаем ApplicationUserRole из объекта с помощью рефлексии
                    applicationUserRole = CreateApplicationUserRoleFromObject(userRole);
                }

                if (_session != null)
                {
                    await _userRoles.InsertOneAsync(_session, applicationUserRole);
                }
                else
                {
                    await _userRoles.InsertOneAsync(applicationUserRole);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания пользовательской роли: {ex.Message}");
            }
        }

        private static ApplicationUserRole CreateApplicationUserRoleFromObject(object userRole)
        {
            var userRoleType = userRole.GetType();
            var applicationUserRole = new ApplicationUserRole();

            var userIdProperty = userRoleType.GetProperty("UserId");
            var roleIdProperty = userRoleType.GetProperty("RoleId");

            if (userIdProperty != null)
                applicationUserRole.UserId = (Guid)userIdProperty.GetValue(userRole)!;
            
            if (roleIdProperty != null)
                applicationUserRole.RoleId = (Guid)roleIdProperty.GetValue(userRole)!;

            applicationUserRole.Id = Guid.NewGuid();

            return applicationUserRole;
        }

        /// <summary>
        /// Получить все пользовательские роли
        /// </summary>
        public IEnumerable<object> GetAllUserRoles()
        {
            var filter = Builders<ApplicationUserRole>.Filter.Empty;
            var userRoles = _session != null 
                ? _userRoles.Find(_session, filter).ToList()
                : _userRoles.Find(filter).ToList();
            return userRoles.Cast<object>();
        }

        public async Task<IUserRole?> GetAsync(Expression<Func<IUserRole, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                // Преобразуем предикат для работы с ApplicationUserRole
                var parameter = Expression.Parameter(typeof(ApplicationUserRole), "ur");
                var visitor = new ParameterReplacer(predicate.Parameters[0], parameter);
                var convertedPredicate = Expression.Lambda<Func<ApplicationUserRole, bool>>(
                    visitor.Visit(predicate.Body), parameter);

                ApplicationUserRole? userRole;
                if (_session != null)
                {
                    userRole = await _userRoles.Find(_session, convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    userRole = await _userRoles.Find(convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }

                return userRole; // ApplicationUserRole реализует IUserRole
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка получения пользовательской роли: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получить роли пользователя по ID пользователя
        /// </summary>
        public async Task<IEnumerable<IUserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationUserRole>.Filter.Eq(ur => ur.UserId, userId);
                var userRoles = _session != null
                    ? await _userRoles.Find(_session, filter).ToListAsync(cancellationToken)
                    : await _userRoles.Find(filter).ToListAsync(cancellationToken);

                return userRoles.Cast<IUserRole>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка получения ролей пользователя: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<IUserRole>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationUserRole>.Filter.Eq(ur => ur.RoleId, roleId);
                var userRoles = _session != null
                    ? await _userRoles.Find(_session, filter).ToListAsync(cancellationToken)
                    : await _userRoles.Find(filter).ToListAsync(cancellationToken);

                return userRoles.Cast<IUserRole>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка получения пользователей роли: {ex.Message}", ex);
            }
        }

        public async Task<Result> UpdateAsync(object userRole)
        {
            try
            {
                if (userRole is not ApplicationUserRole applicationUserRole)
                {
                    // Создаем ApplicationUserRole из объекта с помощью рефлексии
                    applicationUserRole = CreateApplicationUserRoleFromObject(userRole);
                }

                var filter = Builders<ApplicationUserRole>.Filter.And(
                    Builders<ApplicationUserRole>.Filter.Eq(ur => ur.UserId, applicationUserRole.UserId),
                    Builders<ApplicationUserRole>.Filter.Eq(ur => ur.RoleId, applicationUserRole.RoleId)
                );

                var existingUserRole = _session != null 
                    ? await _userRoles.Find(_session, filter).FirstOrDefaultAsync()
                    : await _userRoles.Find(filter).FirstOrDefaultAsync();

                if (existingUserRole == null)
                {
                    return Result.Failure("Пользовательская роль не найдена");
                }

                // Обновляем существующую запись
                var updateFilter = Builders<ApplicationUserRole>.Filter.Eq(ur => ur.Id, existingUserRole.Id);
                if (_session != null)
                {
                    await _userRoles.ReplaceOneAsync(_session, updateFilter, applicationUserRole);
                }
                else
                {
                    await _userRoles.ReplaceOneAsync(updateFilter, applicationUserRole);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка обновления пользовательской роли: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(object userRole, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userRole is not ApplicationUserRole applicationUserRole)
                {
                    // Создаем ApplicationUserRole из объекта с помощью рефлексии
                    applicationUserRole = CreateApplicationUserRoleFromObject(userRole);
                }

                return await DeleteAsync(applicationUserRole.UserId, applicationUserRole.RoleId, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления пользовательской роли: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationUserRole>.Filter.And(
                    Builders<ApplicationUserRole>.Filter.Eq(ur => ur.UserId, userId),
                    Builders<ApplicationUserRole>.Filter.Eq(ur => ur.RoleId, roleId)
                );

                var result = _session != null
                    ? await _userRoles.DeleteOneAsync(_session, filter, cancellationToken: cancellationToken)
                    : await _userRoles.DeleteOneAsync(filter, cancellationToken: cancellationToken);

                if (result.DeletedCount == 0)
                {
                    return Result.Failure("Пользовательская роль не найдена");
                }
               
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления пользовательской роли: {ex.Message}");
            }
        }

        public async Task<Result> DeleteUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationUserRole>.Filter.Eq(ur => ur.UserId, userId);

                var result = _session != null
                    ? await _userRoles.DeleteManyAsync(_session, filter, cancellationToken: cancellationToken)
                    : await _userRoles.DeleteManyAsync(filter, cancellationToken: cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления ролей пользователя: {ex.Message}");
            }
        }

        /// <summary>
        /// Создает пользовательскую роль, предварительно удалив существующие роли пользователя
        /// </summary>
        /// <param name="roleId">ID роли</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result> CreateUserIdRoleIdAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Если запись по userId уже есть, удаляем её
                var existingFilter = Builders<ApplicationUserRole>.Filter.Eq(ur => ur.UserId, userId);
                
                if (_session != null)
                {
                    await _userRoles.DeleteManyAsync(_session, existingFilter, cancellationToken: cancellationToken);
                }
                else
                {
                    await _userRoles.DeleteManyAsync(existingFilter, cancellationToken: cancellationToken);
                }

                // 2. Создаем новую запись в UserRole по входным параметрам
                var newUserRole = new ApplicationUserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = roleId
                };

                if (_session != null)
                {
                    await _userRoles.InsertOneAsync(_session, newUserRole, cancellationToken: cancellationToken);
                }
                else
                {
                    await _userRoles.InsertOneAsync(newUserRole, cancellationToken: cancellationToken);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания пользовательской роли: {ex.Message}");
            }
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}

