using DDD.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DDD.Repositories
{
    //TODO: Добавить токены отмены

    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Добавляет новую сущность.
        /// </summary>
        /// <param name="entity">Сущность для добавления.</param>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет существующую сущность.
        /// </summary>
        /// <param name="entity">Сущность для обновления.</param>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет сущность по идентификатору.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Интерфейс обобщенного репозитория для работы с сущностями.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <typeparam name="TKey">Тип ключа сущности.</typeparam>
    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class, IEntity<TKey>
    {
    }
}