using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Site.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers;

[Route("api/assignments")]
public class AssignmentController(AssignmentService assignmentService) : Controller
{
    private readonly AssignmentService _assignmentService = assignmentService;

    public IActionResult Active()
    {
        return View(nameof(Index));
    }

    public IActionResult Archive()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetAllAssignmentsAsync(null, cancellationToken);
        return Ok(assignments);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(ModelState);
        }

        var assignment = await _assignmentService.GetAssignmentByIdAsync(id, cancellationToken);

        if (assignment is not null) return Ok(assignment);

        HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return NotFound(new { message = "Assignment not found." });
    }

    [HttpPost]
    [Route("active")]
    public async Task<IActionResult> GetActiveAsync([FromServices] DataTableNetService dataTableNetService,
        CancellationToken cancellationToken)
    {
        var input = dataTableNetService.Parse(Request);

        // Преобразуем в доменные параметры
        var parameters = new DataTableParameters
        {
            Skip = input.Skip,
            PageSize = input.PageSize,
            SearchTerm = input.SearchTerm
        };

        // Используем доменный метод репозитория
        var result = await _assignmentService.GetActiveAssignmentsForDataTableAsync(parameters, cancellationToken);

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
    [Route("archive")]
    public async Task<JsonResult> GetAssignmentForDataTableAsync([FromServices] DataTableNetService dataTableNetService,
        CancellationToken cancellationToken = default)
    {
        var input = dataTableNetService.Parse(Request);

        // Преобразуем в доменные параметры
        var parameters = new DataTableParameters
        {
            Skip = input.Skip,
            PageSize = input.PageSize,
            SearchTerm = input.SearchTerm
        };

        // Используем доменный метод репозитория
        var result = await _assignmentService.GetArchivedAssignmentsForDataTableAsync(parameters, cancellationToken);

        // Преобразовать данные для представления
        var dataResult = result.Data.Select(i =>
            new RepresentAssignmentDto()
            {
                Id = i.Id,
                CreateDate = i.CreationDate,
                Description = i.Description
            }
        );

        return new(new
        {
            draw = input.Draw,
            recordsTotal = result.TotalRecords,
            recordsFiltered = result.FilteredRecords,
            data = dataResult
        });
    }

    [HttpPost]
    public async Task<JsonResult> AddAssignmentAsync([FromBody] CreateOrEditAssignmentDto input,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new(ModelState);
        }

        var result = await _assignmentService.CreateAssignmentAsync(input, cancellationToken);

        HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        return new(new { });
    }

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<JsonResult> EditAssignmentAsync([FromRoute] Guid id, [FromBody] CreateOrEditAssignmentDto input,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new(ModelState);
        }

        await _assignmentService.EditAssignmentAsync(id, input, cancellationToken);

        HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        return new(new { });
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<JsonResult> DeletePatientAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new(ModelState);
        }

        await _assignmentService.DeleteAssignmentAsync(id, cancellationToken);

        HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        return new(new { });
    }
}