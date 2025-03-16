namespace DDD.Entities
{
    /// <summary>
    /// Представляет базовый интерфейс для всех сущностей.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Получает массив ключей сущности.
        /// </summary>
        /// <returns>Массив ключей.</returns>
        object?[] GetKeys();
    }

    /// <summary>
    /// Представляет интерфейс сущности с заданным типом ключа.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа сущности.</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Получает идентификатор сущности.
        /// </summary>
        TKey Id { get; }
    }
}