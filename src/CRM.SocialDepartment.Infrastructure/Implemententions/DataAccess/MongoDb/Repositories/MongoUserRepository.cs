using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Helpers;
//using CRM.SocialDepartment.Infrastructure.Identity;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория пользователей
    /// </summary>
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle? _session;
        private readonly IMongoCollection<ApplicationUser> _users;

        public MongoUserRepository(IMongoDatabase database, IClientSessionHandle? session = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _session = session;
            _users = _database.GetCollection<ApplicationUser>("users");
        }

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        public async Task<Result> CreateAsync(object user, string password)
        {
            try
            {
                if (user is not ApplicationUser applicationUser)
                {
                    // Создаем ApplicationUser из объекта с помощью рефлексии
                    applicationUser = CreateApplicationUserFromObject(user);
                }

                // Хешируем пароль перед сохранением
                applicationUser.PasswordHash = PasswordHasher.HashPassword(applicationUser, password);
                if (_session != null)
                {
                    await _users.InsertOneAsync(_session, applicationUser);
                }
                else
                {
                    await _users.InsertOneAsync(applicationUser);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания пользователя: {ex.Message}");
            }
        }

        /// <summary>
        /// Создает ApplicationUser из объекта с помощью рефлексии
        /// </summary>
        private static ApplicationUser CreateApplicationUserFromObject(object user)
        {
            var userType = user.GetType();
            var applicationUser = new ApplicationUser();

            var firstNameProperty = userType.GetProperty("FirstName");
            var lastNameProperty = userType.GetProperty("LastName");
            var userNameProperty = userType.GetProperty("UserName");
            var emailProperty = userType.GetProperty("Email");
            var roleProperty = userType.GetProperty("Role");
            var positionProperty = userType.GetProperty("Position");
            var departmentNumberProperty = userType.GetProperty("DepartmentNumber");

            if (firstNameProperty != null)
                applicationUser.FirstName = firstNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (lastNameProperty != null)
                applicationUser.LastName = lastNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (userNameProperty != null)
                applicationUser.UserName = userNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (emailProperty != null)
                applicationUser.Email = emailProperty.GetValue(user)?.ToString() ?? string.Empty;

            if (roleProperty != null)
                applicationUser.Role = roleProperty.GetValue(user)?.ToString() ?? string.Empty;

            if (positionProperty != null)
                applicationUser.Position = positionProperty.GetValue(user)?.ToString() ?? string.Empty;

            if (departmentNumberProperty != null)
                applicationUser.DepartmentNumber = departmentNumberProperty.GetValue(user)?.ToString() ?? string.Empty;

            applicationUser.Id = Guid.NewGuid(); 
            applicationUser.EmailConfirmed = true; 
            applicationUser.PhoneNumberConfirmed = true; 
            applicationUser.TwoFactorEnabled = false; 
            applicationUser.LockoutEnd = null; 
            applicationUser.AccessFailedCount = 0; 
            applicationUser.LockoutEnabled = false; 
            applicationUser.NormalizedUserName = applicationUser.UserName.ToUpperInvariant();
            applicationUser.NormalizedEmail = applicationUser.Email.ToUpperInvariant();
            applicationUser.SecurityStamp = Guid.NewGuid().ToString();
            applicationUser.ConcurrencyStamp = Guid.NewGuid().ToString();

            return applicationUser;
        }

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        public IEnumerable<object> GetAllUsers()
        {
            var filter = Builders<ApplicationUser>.Filter.Empty;
            var users = _session != null 
                ? _users.Find(_session, filter).ToList()
                : _users.Find(filter).ToList();
            return users.Cast<object>();
        }

        public async Task<Result> VerifyPasswordAsync(string userName, string password)
        {
            try
            {
                var filter = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, userName);
                var user = _session != null 
                    ? await _users.Find(_session, filter).FirstOrDefaultAsync()
                    : await _users.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                {
                    return Result.Failure("Пользователь не найден");
                }

                var verificationResult = PasswordHasher.VerifyPassword(user.PasswordHash, password);
                
                return verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success
                    ? Result.Success()
                    : Result.Failure("Неверный пароль");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка проверки пароля: {ex.Message}");
            }
        }

        public async Task<IUser?> GetAsync(Expression<Func<IUser, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                // Преобразуем предикат для работы с ApplicationUser
                var parameter = Expression.Parameter(typeof(ApplicationUser), "u");
                var visitor = new ParameterReplacer(predicate.Parameters[0], parameter);
                var convertedPredicate = Expression.Lambda<Func<ApplicationUser, bool>>(
                    visitor.Visit(predicate.Body), parameter);

                ApplicationUser? user;
                if (_session != null)
                {
                    user = await _users.Find(_session, convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    user = await _users.Find(convertedPredicate).FirstOrDefaultAsync(cancellationToken);
                }

                return user; // ApplicationUser реализует IUser
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не нарушаем DDD принципы
                throw new InvalidOperationException($"Ошибка получения пользователя: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        public async Task<Result> UpdateAsync(object user, string? password = null)
        {
            try
            {
                if (user is not ApplicationUser applicationUser)
                {
                    // Создаем ApplicationUser из объекта с помощью рефлексии
                    applicationUser = CreateApplicationUserFromObject(user);
                }

                var filter = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, applicationUser.UserName);
                var existingUser = _session != null 
                    ? await _users.Find(_session, filter).FirstOrDefaultAsync()
                    : await _users.Find(filter).FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    return Result.Failure("Пользователь не найден");
                }

                existingUser.FirstName = applicationUser.FirstName;
                existingUser.LastName = applicationUser.LastName;
                existingUser.Email = applicationUser.Email;
                existingUser.Role = applicationUser.Role;
                existingUser.Position = applicationUser.Position;
                existingUser.DepartmentNumber = applicationUser.DepartmentNumber;
                existingUser.NormalizedEmail = applicationUser.Email.ToUpperInvariant();

                if (!string.IsNullOrEmpty(password))
                {
                    existingUser.PasswordHash = PasswordHasher.HashPassword(existingUser, password);
                }

                var updateFilter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, existingUser.Id);
                if (_session != null)
                {
                    await _users.ReplaceOneAsync(_session, updateFilter, existingUser);
                }
                else
                {
                    await _users.ReplaceOneAsync(updateFilter, existingUser);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка обновления пользователя: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(object user, CancellationToken cancellationToken = default)
        {
            try
            {
                if (user is not ApplicationUser applicationUser)
                {
                    // Создаем ApplicationUser из объекта с помощью рефлексии
                    applicationUser = CreateApplicationUserFromObject(user);
                }

                return await DeleteAsync(applicationUser.UserName, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления пользователя: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(string userName, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, userName);
                var existingUser = _session != null
                    ? await _users.Find(_session, filter).FirstOrDefaultAsync()
                    : await _users.Find(filter).FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    return Result.Failure("Пользователь не найден");
                }

                var deleteFilter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, existingUser.Id);
                await _users.DeleteOneAsync(deleteFilter, cancellationToken);
               
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления пользователя: {ex.Message}");
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