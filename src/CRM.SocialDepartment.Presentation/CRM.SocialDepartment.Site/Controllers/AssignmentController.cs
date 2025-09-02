using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.Site.ViewModels.Assignment;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers;

public class AssignmentController(AssignmentService assignmentService) : Controller
{
    [HttpGet]
    [Route("assignments")]
    public IActionResult Index()
    {
        return View("~/Views/Assignment/Index.cshtml");
    }

    [HttpGet]
    [Route("assignments/archive")]
    public IActionResult Archive()
    {
        return View("~/Views/Assignment/Archive.cshtml");
    }

    [HttpGet]
    [Route("[controller]/modal/create")]
    public IActionResult CreateModal()
    {
        ViewData.Model = new CreateAssignmentViewModel()
        {
            Name = "Название",
            AcceptDate = DateTime.UtcNow,
            Description = "Описание",
            Assignee = "Исполнитель",
            PatientId = "Пациент"
        };

        return new PartialViewResult
        {
            ViewName = "~/Views/Assignment/_CreateAssignmentModal.cshtml",
            ViewData = ViewData
        };
    }

    [HttpGet]
    [Route("[controller]/modal/edit")]
    public async Task<IActionResult> EditModal([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var dto = await assignmentService.GetAssignmentByIdAsync(id, cancellationToken);
        if (dto == null)
            return NotFound();

        ViewData.Model = new EditAssignmentViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedDate = dto.CreationDate,
            ForwardDepartment = dto.ForwardDepartment,
            Assignee = dto.Assignee,
            AcceptDate = dto.AcceptDate,
            ForwardDate = dto.ForwardDate,
            DepartmentNumber = dto.DepartmentNumber,
            DepartmentForwardDate = dto.DepartmentForwardDate,
            Note = dto.Note,
            PatientId = dto.PatientId
        };

        return new PartialViewResult
        {
            ViewName = "~/Views/Assignment/_EditAssignmentModal.cshtml",
            ViewData = ViewData
        };
    }

    [HttpPost]
    [Route("api/assignments/active")]
    public async Task<JsonResult> GetActiveForDataTableAsync([FromServices] DataTableNetService dataTableNetService,
        CancellationToken cancellationToken = default)
    {
        var input = dataTableNetService.Parse(Request);

        // Преобразуем в доменные параметры
        var parameters = new DataTableParameters
        {
            Skip = input.Skip,
            PageSize = input.PageSize,
            SearchTerm = input.SearchTerm,
        };

        // Используем доменный метод репозитория
        var result = await assignmentService.GetActiveAssignmentsForDataTableAsync(parameters, cancellationToken);

        // Преобразовать данные для представления
        var dataResult = result.Data.Select(x => new RepresentAssignmentDto
            {
                Id = x.Id,
                Description = x.Description,
                CreateDate = x.CreationDate,
            }
        );

        return new JsonResult(new
        {
            draw = input.Draw,
            recordsTotal = result.TotalRecords,
            recordsFiltered = result.FilteredRecords,
            data = dataResult
        });
    }

    [HttpPost]
    [Route("api/assignments/archive")]
    public async Task<JsonResult> GetArchivedForDataTableAsync([FromServices] DataTableNetService dataTableNetService,
        CancellationToken cancellationToken = default)
    {
        var input = dataTableNetService.Parse(Request);

        // Преобразуем в доменные параметры
        var parameters = new DataTableParameters
        {
            Skip = input.Skip,
            PageSize = input.PageSize,
            SearchTerm = input.SearchTerm,
        };

        // Используем доменный метод репозитория
        var result = await assignmentService.GetActiveAssignmentsForDataTableAsync(parameters, cancellationToken);

        // Преобразовать данные для представления
        var dataResult = result.Data.Where(x => x.IsArchive).Select(x => new RepresentAssignmentDto
            {
                Id = x.Id,
                Description = x.Description,
                CreateDate = x.CreationDate,
            }
        );

        return new JsonResult(new
        {
            draw = input.Draw,
            recordsTotal = result.TotalRecords,
            recordsFiltered = result.FilteredRecords,
            data = dataResult
        });
    }
    
    [HttpDelete]
    [Route("api/assignments/{id:guid}")]
    public async Task<JsonResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { error = "Invalid id" });
        }

        await assignmentService.DeleteAssignmentAsync(id, cancellationToken);
        HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        return new JsonResult(new { });
    }
}