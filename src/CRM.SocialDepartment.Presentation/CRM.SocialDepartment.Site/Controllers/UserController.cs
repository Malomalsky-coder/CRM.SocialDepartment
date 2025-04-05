using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Site.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace CRM.SocialDepartment.Site.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Active()
        {
            return View(nameof(Index));
        }

        public JsonResult GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            var result = users.Select(i =>
            {
                return new
                {
                    i.UserName,
                    i.Email,
                    i.CreatedOn
            };
            });

            var userCount = users.Count();

            return new JsonResult(new
            {
                recordsTotal = userCount,
                recordsFiltered = userCount,
                data = result
            });
        }
    }
}
