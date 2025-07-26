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
        protected readonly IClientSessionHandle? _session;

        public MongoBasicRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
            _session = null;
        }

        protected MongoBasicRepository(IMongoDatabase database, string collectionName, IClientSessionHandle? session)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
            _session = session;
        }

        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _collection.InsertOneAsync(_session, entity, new InsertOneOptions(), cancellationToken);
            }
            else
            {
                await _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
            }
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _collection.ReplaceOneAsync(_session, e => e.Id!.Equals(entity.Id), entity, cancellationToken: cancellationToken);
            }
            else
            {
                await _collection.ReplaceOneAsync(e => e.Id!.Equals(entity.Id), entity, cancellationToken: cancellationToken);
            }
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _collection.DeleteOneAsync(_session, e => e.Id!.Equals(entity.Id), cancellationToken: cancellationToken);
            }
            else
            {
                await _collection.DeleteOneAsync(e => e.Id!.Equals(entity.Id), cancellationToken);
            }
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                return await _collection.Find(_session, predicate).FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                return predicate == null
                    ? await _collection.Find(_session, _ => true).ToListAsync(cancellationToken)
                    : await _collection.Find(_session, predicate).ToListAsync(cancellationToken);
            }
            else
            {
                return predicate == null
                    ? await _collection.Find(_ => true).ToListAsync(cancellationToken)
                    : await _collection.Find(predicate).ToListAsync(cancellationToken);
            }
        }

        public IMongoCollection<TEntity> GetCollection()
        {
            return _collection;
        }
    }
}
