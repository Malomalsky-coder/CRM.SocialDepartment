using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace CRM.SocialDepartment.Infrastructure.Identity
{
    public class UserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result> CreateAsync(ApplicationUser user, string password)
        {
            var identityResult = await _userManager.CreateAsync(user, password);

            if (!identityResult.Succeeded)
            {
                var error = string.Join(", ", identityResult.Errors.Select(e => e.Description));

                return Result.Failure(error);
            }

            return Result.Success();
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }
    }
}
