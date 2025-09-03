namespace CRM.SocialDepartment.Domain.Entities
{
    /// <summary>
    /// Интерфейс роли для доменного слоя
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        Guid Id { get; }
        
        string  Name { get; set; }
        
        /// <summary>
        /// Получает идентификатор роли
        /// </summary>
        /// <returns>Идентификатор роли</returns>
        Guid GetId();
    }
}


