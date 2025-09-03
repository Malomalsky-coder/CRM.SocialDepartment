using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория ролей
    /// </summary>
    public class MongoRoleRepository : IRoleRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle? _session;
        private readonly IMongoCollection<ApplicationRole> _roles;

        public MongoRoleRepository(IMongoDatabase database, IClientSessionHandle? session = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _session = session;
            _roles = _database.GetCollection<ApplicationRole>("roles");
        }

        /// <summary>
        /// Создать новую роль
        /// </summary>
        public async Task<Result> CreateAsync(object role)
        {
            try
            {
                if (role is not ApplicationRole applicationRole)
                {
                    // Создаем ApplicationRole из объекта с помощью рефлексии
                    applicationRole = CreateApplicationRoleFromObject(role);
                }

                if (_session != null)
                {
                    await _roles.InsertOneAsync(_session, applicationRole);
                }
                else
                {
                    await _roles.InsertOneAsync(applicationRole);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания роли: {ex.Message}");
            }
        }

        /// <summary>
        /// Создает ApplicationRole из объекта с помощью рефлексии
        /// </summary>
        private static ApplicationRole CreateApplicationRoleFromObject(object role)
        {
            var roleType = role.GetType();
            var applicationRole = new ApplicationRole();

            var nameProperty = roleType.GetProperty("Name");
            var descriptionProperty = roleType.GetProperty("Description");

            if (nameProperty != null)
                applicationRole.Name = nameProperty.GetValue(role)?.ToString() ?? string.Empty;
            

            applicationRole.Id = Guid.NewGuid();
            applicationRole.NormalizedName = applicationRole.Name.ToUpperInvariant();
            applicationRole.ConcurrencyStamp = Guid.NewGuid().ToString();
            applicationRole.CreatedOn = DateTime.UtcNow;

            return applicationRole;
        }

        /// <summary>
        /// Получить все роли
        /// </summary>
        public IEnumerable<object> GetAllRoles()
        {
            var filter = Builders<ApplicationRole>.Filter.Empty;
            var roles = _session != null 
                ? _roles.Find(_session, filter).ToList()
                : _roles.Find(filter).ToList();
            return roles.Cast<object>();
        }

        public async Task<IRole?> GetAsync(Expression<Func<IRole, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                // Преобразуем предикат для работы с ApplicationRole
                var parameter = Expression.Parameter(typeof(ApplicationRole), "r");
                var visitor = new ParameterReplacer(predicate.Parameters[0], parameter);
                var convertedPredicate = Expression.Lambda<Func<ApplicationRole, bool>>(
                    visitor.Visit(predicate.Body), parameter);

                ApplicationRole? role;
                if (_session != null)
                {
                    role = await _roles.Find(_session, convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    role = await _roles.Find(convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }

                return role; 
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не нарушаем DDD принципы
                throw new InvalidOperationException($"Ошибка получения роли: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Обновить роль
        /// </summary>
        public async Task<Result> UpdateAsync(object role)
        {
            try
            {
                if (role is not ApplicationRole applicationRole)
                {
                    // Создаем ApplicationRole из объекта с помощью рефлексии
                    applicationRole = CreateApplicationRoleFromObject(role);
                }

                var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Name, applicationRole.Name);
                var existingRole = _session != null 
                    ? await _roles.Find(_session, filter).FirstOrDefaultAsync()
                    : await _roles.Find(filter).FirstOrDefaultAsync();

                if (existingRole == null)
                {
                    return Result.Failure("Роль не найдена");
                }

                existingRole.Name = applicationRole.Name;
                existingRole.NormalizedName = applicationRole.Name.ToUpperInvariant();

                var updateFilter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, existingRole.Id);
                if (_session != null)
                {
                    await _roles.ReplaceOneAsync(_session, updateFilter, existingRole);
                }
                else
                {
                    await _roles.ReplaceOneAsync(updateFilter, existingRole);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка обновления роли: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(object role, CancellationToken cancellationToken = default)
        {
            try
            {
                if (role is not ApplicationRole applicationRole)
                {
                    // Создаем ApplicationRole из объекта с помощью рефлексии
                    applicationRole = CreateApplicationRoleFromObject(role);
                }

                return await DeleteAsync(applicationRole.Name, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления роли: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Name, roleName);
                var existingRole = _session != null
                    ? await _roles.Find(_session, filter).FirstOrDefaultAsync()
                    : await _roles.Find(filter).FirstOrDefaultAsync();

                if (existingRole == null)
                {
                    return Result.Failure("Роль не найдена");
                }

                var deleteFilter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, existingRole.Id);
                await _roles.DeleteOneAsync(deleteFilter, cancellationToken);
               
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления роли: {ex.Message}");
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

