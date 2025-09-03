using AspNetCore.Identity.MongoDbCore.Models;
using DDD.Entities;
using CRM.SocialDepartment.Domain.Entities;
using MongoDbGenericRepository.Attributes;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    [CollectionName("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>, IEntity<Guid>, IUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string DepartmentNumber { get; set; } = string.Empty;
        
        public object?[] GetKeys()
        {
            return new object?[] { UserName };
        }
        
        /// <summary>
        /// Получает идентификатор пользователя
        /// </summary>
        /// <returns>Идентификатор пользователя</returns>
        public Guid GetId()
        {
            return Id;
        }
    }
}
