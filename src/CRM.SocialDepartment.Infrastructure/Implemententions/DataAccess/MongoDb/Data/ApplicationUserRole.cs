using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using CRM.SocialDepartment.Domain.Entities;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    [CollectionName("userRoles")]
    public class ApplicationUserRole : IUserRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
