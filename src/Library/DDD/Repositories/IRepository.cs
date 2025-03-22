using DDD.Entities;
using System.Threading.Tasks;

namespace DDD.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Добавляет новую сущность.
        /// </summary>
        /// <param name="entity">Сущность для добавления.</param>
        Task InsertAsync(TEntity entity);

        /// <summary>
        /// Обновляет существующую сущность.
        /// </summary>
        /// <param name="entity">Сущность для обновления.</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Удаляет сущность по идентификатору.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        Task DeleteAsync(TEntity entity);
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