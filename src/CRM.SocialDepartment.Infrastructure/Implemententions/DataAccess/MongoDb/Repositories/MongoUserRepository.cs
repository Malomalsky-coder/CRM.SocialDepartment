using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Helpers;
//using CRM.SocialDepartment.Infrastructure.Identity;
using MongoDB.Driver;
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

            if (firstNameProperty != null)
                applicationUser.FirstName = firstNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (lastNameProperty != null)
                applicationUser.LastName = lastNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (userNameProperty != null)
                applicationUser.UserName = userNameProperty.GetValue(user)?.ToString() ?? string.Empty;
            
            if (emailProperty != null)
                applicationUser.Email = emailProperty.GetValue(user)?.ToString() ?? string.Empty;

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
    }
} 