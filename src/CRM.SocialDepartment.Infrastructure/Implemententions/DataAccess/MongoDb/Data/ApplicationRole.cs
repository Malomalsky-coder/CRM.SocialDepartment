using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using CRM.SocialDepartment.Domain.Entities;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    [CollectionName("roles")]
    public class ApplicationRole : MongoIdentityRole<Guid>, IRole
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Получает идентификатор роли
        /// </summary>
        /// <returns>Идентификатор роли</returns>
        public Guid GetId()
        {
            return Id;
        }
    }
}
