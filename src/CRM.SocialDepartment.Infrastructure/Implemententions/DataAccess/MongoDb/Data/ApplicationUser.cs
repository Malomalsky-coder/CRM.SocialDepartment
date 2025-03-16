using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    [CollectionName("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }

}
