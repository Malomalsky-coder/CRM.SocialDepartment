using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
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

                // Здесь должна быть логика хеширования пароля
                // Пока просто сохраняем пользователя
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

            // Безопасно получаем значения свойств через рефлексию
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
    }
} 