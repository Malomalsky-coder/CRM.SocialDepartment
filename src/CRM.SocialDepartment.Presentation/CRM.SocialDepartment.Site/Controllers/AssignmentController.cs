using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CRM.SocialDepartment.Application.Assignments;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Site.Models;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.Site.ViewModels.Assignment;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers;

public class AssignmentController(
    AssignmentService assignmentService,
    ILogger<AssignmentController> logger,
    IMapper mapper)
    : Controller
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

    [HttpPost]
    [Route("api/assignments")]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateAsync(CreateAssignmentViewModel input, CancellationToken cancellationToken)
    {
        var modelStateErrorsToRemove = ModelState.Select(modelError => modelError.Key).Where(fieldName =>
                fieldName.StartsWith("Name.") || fieldName.StartsWith("Description.") ||
                fieldName.StartsWith("Assignee.") || fieldName.StartsWith("DepartmentNumber.") ||
                fieldName.StartsWith("Note"))
            .ToList();

        foreach (var fieldName in modelStateErrorsToRemove)
        {
            ModelState.Remove(fieldName);
        }

        // Теперь проверяем только ручную валидацию
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(input);

        // Вызываем ручную валидацию
        var manualValidationResults = input.Validate(validationContext);
        validationResults.AddRange(manualValidationResults);

        if (!ModelState.IsValid)
        {
            foreach (var modelError in ModelState)
            {
                foreach (var error in modelError.Value.Errors)
                {
                    var fieldName = modelError.Key;
                    var errorMessage = error.ErrorMessage;

                    // Обрабатываем только базовые поля
                    if (fieldName == "Name")
                    {
                        validationResults.Add(new ValidationResult(errorMessage, [fieldName]));
                    }
                    // Улучшаем сообщения об ошибках для других полей
                    else if (errorMessage == "The value '' is invalid.")
                    {
                        var improvedMessage = GetDetailedErrorMessage(fieldName);
                        validationResults.Add(new ValidationResult(improvedMessage, [fieldName]));
                    }
                    else if (string.IsNullOrEmpty(errorMessage))
                    {
                        var improvedMessage = $"Ошибка в поле '{fieldName}' (без сообщения)";
                        validationResults.Add(new ValidationResult(improvedMessage, [fieldName]));
                    }
                }
            }
        }

        if (validationResults.Count != 0)
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

            logger.LogWarning("❌ [AssignmentController] Возвращаем ошибки валидации: {Errors}",
                string.Join(", ", errors));
            return new JsonResult(ApiResponse<object>.Error("Неверные данные", new
            {
                Errors = errors
            }))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        try
        {
            var dto = mapper.Map<CreateOrEditAssignmentDto>(input);

            logger.LogInformation("💾 [AssignmentController] Сохранение пациента в базу данных");
            var result = await assignmentService.CreateAssignmentAsync(dto, cancellationToken);

            logger.LogInformation("✅ [AssignmentController] Пациент успешно создан с ID: {PatientId}", result);
            return new JsonResult(ApiResponse<Guid>.Ok(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🚨 [AssignmentController] Ошибка при создании пациента");
            throw;
        }
    }

    private static string GetDetailedErrorMessage(string fieldName)
    {
        return fieldName switch
        {
            "Name" => "Название задачи пустое или слишком короткое",
            "Description" => "Описание задачи пустое или слишком короткое",
            "Assignee" => "Исполнитель не назначен",
            "DepartmentNumber" => "Номер отдела не введён",
            _ => $"Введено некорректное значение в поле '{fieldName}'"
        };
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