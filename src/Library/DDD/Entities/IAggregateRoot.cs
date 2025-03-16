namespace DDD.Entities
{
    /// <summary>
    /// Представляет базовый интерфейс для корней агрегатов.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
    }

    /// <summary>
    /// Представляет интерфейс корня агрегата с заданным типом ключа.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа корня агрегата.</typeparam>
    public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
    {
    }
}