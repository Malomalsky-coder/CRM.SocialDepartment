using AspNetCore.Identity.MongoDbCore.Models;
using DDD.Entities;
using MongoDbGenericRepository.Attributes;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    [CollectionName("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>, IEntity<Guid>
    {
        public object?[] GetKeys()
        {
            return new object?[] { Id };
        }
    }

}
