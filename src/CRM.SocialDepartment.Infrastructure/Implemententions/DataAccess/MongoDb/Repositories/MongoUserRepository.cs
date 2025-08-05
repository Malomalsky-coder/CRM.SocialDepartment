using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
//using CRM.SocialDepartment.Infrastructure.Identity;
using MongoDB.Driver;

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
                    // Создаем ApplicationUser из анонимного объекта
                    var userData = (dynamic)user;
                    applicationUser = new ApplicationUser
                    {
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        UserName = userData.UserName,
                        Email = userData.Email
                    };
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