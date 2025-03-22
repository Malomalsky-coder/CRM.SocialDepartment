using DDD.Entities;

namespace DDD.Repositories
{
    public interface IBasicRepository<TEntity> : IRepository<TEntity>, IReadOnlyRepository<TEntity>
        where TEntity : class, IEntity
    {
    }

    public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
    }
}
