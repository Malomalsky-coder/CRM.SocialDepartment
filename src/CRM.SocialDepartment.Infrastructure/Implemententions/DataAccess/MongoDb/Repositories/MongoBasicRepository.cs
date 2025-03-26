using DDD.Entities;
using DDD.Repositories;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    public class MongoBasicRepository<TEntity, TKey> : IBasicRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoBasicRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        [Obsolete("Использую устаревший метод MongoDB.Driver")]
        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(e => e.Id!.Equals(entity.Id), entity, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(e => e.Id!.Equals(entity.Id), cancellationToken);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await _collection.Find(_ => true).ToListAsync(cancellationToken)
                : await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public IMongoCollection<TEntity> GetCollection()
        {
            return _collection;
        }
    }
}
