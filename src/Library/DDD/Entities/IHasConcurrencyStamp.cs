namespace DDD.Entities
{
    /// <summary>
    /// Представляет интерфейс для объектов, которые имеют метку конкурентности.
    /// </summary>
    public interface IHasConcurrencyStamp
    {
        /// <summary>
        /// Получает или устанавливает метку конкурентности.
        /// </summary>
        string ConcurrencyStamp { get; set; }
    }
}