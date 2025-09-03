using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CRM.SocialDepartment.Site.ViewModels.Role;
using CRM.SocialDepartment.Application.Roles;
using CRM.SocialDepartment.Application.DTOs;

namespace CRM.SocialDepartment.Site.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleAppService _roleAppService;
        private readonly IMapper _mapper;

        public RoleController(RoleAppService roleAppService, IMapper mapper)
        {
            _roleAppService = roleAppService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/Role/GetAllRoles")]
        public JsonResult GetAllRoles()
        {
            var roles = _roleAppService.GetAllRoles();

            var roleCount = roles.Count();

            return new JsonResult(new
            {
                recordsTotal = roleCount,
                recordsFiltered = roleCount,
                data = roles
            });
        }

        [HttpGet]
        [Route("[controller]/modal/create")]
        public IActionResult GetCreateForm()
        {
            ViewData.Model = new CreateRoleViewModel();
            return new PartialViewResult
            {
                ViewName = $"~/Views/Role/_CreateRoleModal.cshtml",
                ViewData = ViewData
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<CreateRoleDTO>(model);
                var result = await _roleAppService.CreateRoleAsync(dto);

                return result.IsSuccess
                    ? Ok("Роль добавлена")
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

        [HttpGet]
        [Route("[controller]/modal/edit")]
        public async Task<IActionResult> GetEditRoleModalAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var role = await _roleAppService.GetRoleByNameAsync(roleName, cancellationToken);

            if (role is null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<EditRoleViewModel>(role);

            ViewData.Model = viewModel;
            return new PartialViewResult
            {
                ViewName = $"~/Views/Role/_EditRoleModal.cshtml",
                ViewData = ViewData
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoleAsync(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<CreateRoleDTO>(model);
                var result = await _roleAppService.UpdateRoleAsync(dto);

                return result.IsSuccess
                    ? Ok("Роль обновлена")
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

        [HttpPost]
        [Route("[controller]/delete")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody] DeleteRoleRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request?.RoleName))
            {
                return BadRequest(new { Message = "Название роли не указано" });
            }

            try
            {
                var result = await _roleAppService.DeleteRoleAsync(request.RoleName, cancellationToken);

                return result.IsSuccess
                    ? Ok(new { Message = "Роль удалена" })
                    : BadRequest(new { Message = result.Errors });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Произошла ошибка при удалении роли", Error = ex.Message });
            }
        }
    }

    public class DeleteRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
    }
}
