using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Application.Users;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Site.Helpers;
using CRM.SocialDepartment.Site.Models;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.Site.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Mail;


namespace CRM.SocialDepartment.Site.Controllers
{
    public class UserController : Controller
    {
        private readonly UserAppService _userAppService;
        private readonly IMapper _mapper;

        public UserController(UserAppService userAppService, IMapper mapper)
        {
            _userAppService = userAppService;
            _mapper = mapper;
        }

        public IActionResult Active()
        {
            return View(nameof(Index));
        }

        public JsonResult GetAllUsers()
        {
            var users = _userAppService.GetAllUsers();

            var userCount = users.Count();

            return new JsonResult(new
            {
                recordsTotal = userCount,
                recordsFiltered = userCount,
                data = users
            });
        }

        [HttpGet]
        [Route("[controller]/modal/create")]
        public IActionResult GetCreateForm()
        {
            ViewData.Model = new CreateUserViewModel();
            return new PartialViewResult
            {
                ViewName = $"~/Views/User/_CreateUserModal.cshtml",
                ViewData = ViewData
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<CreateUserDTO>(model);
                var result = await _userAppService.CreateUserAsync(dto);

                return result.IsSuccess
                    ? Ok("Пользователь добавлен")
                    : BadRequest($"{result.Errors}");
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                return BadRequest(new { Message = "Ошибка валидации ", Errors = errorList });
            }
        }
    }
}
