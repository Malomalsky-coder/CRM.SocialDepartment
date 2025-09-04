using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers;

public class UsersController : Controller
{
    [HttpGet]
    [Route("api/users")]
    public async Task<JsonResult> GetUsers([FromServices] IServiceProvider services, CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = services.GetService<UserManager<IdentityUser>>();
            if (userManager != null)
            {
                var users = await Task.Run(() => userManager.Users
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName ?? string.Empty,
                        fullName = u.UserName ?? string.Empty
                    })
                    .ToList(), cancellationToken);

                return new JsonResult(users);
            }
        }
        catch
        {
        }

        var fallback = new[]
        {
            new { id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), userName = "user1", fullName = "Пользователь 1" },
            new { id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), userName = "user2", fullName = "Пользователь 2" },
            new { id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), userName = "user3", fullName = "Пользователь 3" }
        };

        return new JsonResult(fallback);
    }
}
