namespace CRM.SocialDepartment.Domain.Entities
{
    /// <summary>
    /// Интерфейс пользователя для доменного слоя
    /// </summary>
    public interface IUser
    {
        string UserName { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        DateTime CreatedOn { get; }
        string Role { get; }
        string Position { get; }
        string DepartmentNumber { get; }
        
        /// <summary>
        /// Получает идентификатор пользователя
        /// </summary>
        /// <returns>Идентификатор пользователя</returns>
        Guid GetId();
    }
} 