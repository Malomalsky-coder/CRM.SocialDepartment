using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Repositories;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория логов активности пользователей
    /// </summary>
    public class MongoUserActivityLogRepository : IUserActivityLogRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle? _session;
        private readonly IMongoCollection<UserActivityLog> _logs;

        public MongoUserActivityLogRepository(IMongoDatabase database, IClientSessionHandle? session = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _session = session;
            _logs = _database.GetCollection<UserActivityLog>("user_activity_logs");
        }

        /// <summary>
        /// Добавить лог активности
        /// </summary>
        public async Task<Result> AddAsync(UserActivityLog log)
        {
            try
            {
                if (_session != null)
                {
                    await _logs.InsertOneAsync(_session, log);
                }
                else
                {
                    await _logs.InsertOneAsync(log);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка добавления лога активности: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить все логи активности
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetAllAsync()
        {
            var filter = Builders<UserActivityLog>.Filter.Empty;
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);
            
            return await _logs.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Получить логи активности пользователя
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(Guid userId)
        {
            var filter = Builders<UserActivityLog>.Filter.Eq(x => x.UserId, userId);
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);
            
            return await _logs.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Получить логи активности по типу действия
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetByActivityTypeAsync(UserActivityType activityType)
        {
            var filter = Builders<UserActivityLog>.Filter.Eq(x => x.ActivityType, activityType);
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);
            
            return await _logs.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Получить логи активности по диапазону дат
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<UserActivityLog>.Filter.And(
                Builders<UserActivityLog>.Filter.Gte(x => x.Timestamp, startDate),
                Builders<UserActivityLog>.Filter.Lte(x => x.Timestamp, endDate)
            );
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);
            
            return await _logs.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Получить логи активности по сущности
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetByEntityAsync(string entityType, Guid entityId)
        {
            var filter = Builders<UserActivityLog>.Filter.And(
                Builders<UserActivityLog>.Filter.Eq(x => x.EntityType, entityType),
                Builders<UserActivityLog>.Filter.Eq(x => x.EntityId, entityId)
            );
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);
            
            return await _logs.Find(filter).Sort(sort).ToListAsync();
        }

        /// <summary>
        /// Получить логи активности с фильтрацией
        /// </summary>
        public async Task<IEnumerable<UserActivityLog>> GetFilteredAsync(
            Guid? userId = null,
            UserActivityType? activityType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? entityType = null,
            int? page = null,
            int? pageSize = null)
        {
            var filterBuilder = Builders<UserActivityLog>.Filter;
            var filters = new List<FilterDefinition<UserActivityLog>>();

            if (userId.HasValue)
            {
                filters.Add(filterBuilder.Eq(x => x.UserId, userId.Value));
            }

            if (activityType.HasValue)
            {
                filters.Add(filterBuilder.Eq(x => x.ActivityType, activityType.Value));
            }

            if (startDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(x => x.Timestamp, startDate.Value));
            }

            if (endDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(x => x.Timestamp, endDate.Value));
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                filters.Add(filterBuilder.Eq(x => x.EntityType, entityType));
            }

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
            var sort = Builders<UserActivityLog>.Sort.Descending(x => x.Timestamp);

            var query = _logs.Find(filter).Sort(sort);

            if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
            {
                var skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Limit(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Получить количество логов с фильтрацией
        /// </summary>
        public async Task<int> GetCountAsync(
            Guid? userId = null,
            UserActivityType? activityType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? entityType = null)
        {
            var filterBuilder = Builders<UserActivityLog>.Filter;
            var filters = new List<FilterDefinition<UserActivityLog>>();

            if (userId.HasValue)
            {
                filters.Add(filterBuilder.Eq(x => x.UserId, userId.Value));
            }

            if (activityType.HasValue)
            {
                filters.Add(filterBuilder.Eq(x => x.ActivityType, activityType.Value));
            }

            if (startDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(x => x.Timestamp, startDate.Value));
            }

            if (endDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(x => x.Timestamp, endDate.Value));
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                filters.Add(filterBuilder.Eq(x => x.EntityType, entityType));
            }

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
            
            return (int)await _logs.CountDocumentsAsync(filter);
        }

        /// <summary>
        /// Удалить старые логи (старше указанной даты)
        /// </summary>
        public async Task<Result> DeleteOldLogsAsync(DateTime olderThan)
        {
            try
            {
                var filter = Builders<UserActivityLog>.Filter.Lt(x => x.Timestamp, olderThan);
                
                if (_session != null)
                {
                    await _logs.DeleteManyAsync(_session, filter);
                }
                else
                {
                    await _logs.DeleteManyAsync(filter);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка удаления старых логов: {ex.Message}");
            }
        }
    }
}
