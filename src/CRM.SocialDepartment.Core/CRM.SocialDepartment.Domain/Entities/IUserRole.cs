namespace CRM.SocialDepartment.Domain.Entities
{
    /// <summary>
    /// Интерфейс пользовательской роли для доменного слоя
    /// </summary>
    public interface IUserRole
    {
        Guid UserId { get; }
        Guid RoleId { get; }
    }
}




