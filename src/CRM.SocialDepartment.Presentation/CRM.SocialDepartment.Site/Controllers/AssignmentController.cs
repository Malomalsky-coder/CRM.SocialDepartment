using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Site.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
        var assignments = _assignmentService.GetAllAssignmentsAsync(null, cancellationToken);
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

        var filter = Builders<Assignment>.Filter.Or(
            Builders<Assignment>.Filter.Where(i => i.SoftDeleted),
            Builders<Assignment>.Filter.Where(i => !i.IsArchive));

        if (!string.IsNullOrEmpty(input.SearchTerm))
        {
            var searchTerm = input.SearchTerm.ToLower();
            filter = Builders<Assignment>.Filter.Or(
                Builders<Assignment>.Filter.Where(i =>
                    i.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)));
        }

        var totalRecords = await _assignmentService.GetAssignmentCollection()
            .CountDocumentsAsync(Builders<Assignment>.Filter.Empty, cancellationToken: cancellationToken);

        var filteredRecords = await _assignmentService.GetAssignmentCollection()
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var assignments = await _assignmentService.GetAssignmentCollection().Find(filter)
            .Skip(input.Skip)
            .Limit(input.PageSize)
            .ToListAsync(cancellationToken);

        var result = assignments.Select(x => new RepresentAssignmentDto
            {
                Id = x.Id,
                Description = x.Description,
                CreateDate = x.CreationDate,
            }
        );

        return new JsonResult(new
        {
            draw = input.Draw,
            recordsTotal = totalRecords,
            recordsFiltered = filteredRecords,
            data = result
        });
    }
}