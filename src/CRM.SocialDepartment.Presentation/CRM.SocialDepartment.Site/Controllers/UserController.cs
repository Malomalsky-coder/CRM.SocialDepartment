using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Site.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Mail;


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
                    i.FirstName,
                    i.LastName,
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

        public IActionResult GetCreateForm()
        {
            return PartialView("_CreateModal", new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            string errors = "";

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = new MailAddress(model.Email).User;
                user.Email = model.Password;
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Пользователь добавлен" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        errors += ", " + error.Description;
                    }
                }
            }

            return Json(new { success = false, message = "Ошибка валидации :" + errors, errors = ModelState.Values.SelectMany(v => v.Errors)});
        }

        
    }
}
