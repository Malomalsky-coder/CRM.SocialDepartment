using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class UserDTO //(string? UserName, string FirstName, string LastName, string? Email, DateTime CreatedOn);
    {
        public string UserName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public DateTime CreatedOn { get; init; }
    }
    
}
