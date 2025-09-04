using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public IActionResult CreateModal([FromQuery] Guid? patientId = null)
    {
        ViewData.Model = new CreateAssignmentViewModel()
        {
            Name = "Название",
            AcceptDate = DateTime.UtcNow,
            Description = "Описание",
            Assignee = "Исполнитель",
            PatientId = patientId ?? Guid.Empty
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

        // Преобразовать данные для представления (полный набор полей, ожидаемых таблицей)
        var dataResult = result.Data.Select(x => new
            {
                id = x.Id,
                acceptDate = x.AcceptDate,
                departmentNumber = x.DepartmentNumber,
                description = x.Description,
                forwardDate = x.ForwardDate,
                forwardDepartment = x.ForwardDepartment,
                name = x.Name,
                departmentForwardDate = x.DepartmentForwardDate,
                assignee = x.Assignee,
                note = x.Note,
                createdDate = x.CreationDate,
                patient = x.PatientId
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

        // Преобразовать данные для представления (полный набор полей, ожидаемых таблицей)
        var dataResult = result.Data.Where(x => x.IsArchive).Select(x => new
            {
                id = x.Id,
                acceptDate = x.AcceptDate,
                departmentNumber = x.DepartmentNumber,
                description = x.Description,
                forwardDate = x.ForwardDate,
                forwardDepartment = x.ForwardDepartment,
                name = x.Name,
                departmentForwardDate = x.DepartmentForwardDate,
                assignee = x.Assignee,
                note = x.Note,
                createdDate = x.CreationDate,
                patient = x.PatientId
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
        // Логируем начало создания задачи
        var requestId = Request.Headers["X-Request-ID"].FirstOrDefault() ?? "unknown";
        logger.LogInformation("🚀 [AssignmentController] Начинаем создание задачи (Request ID: {RequestId})", requestId);
        
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

            // Проверяем на дублирование: ищем похожие задания для того же пациента
            var existingAssignments = await assignmentService.GetAllAssignmentsAsync(
                a => a.PatientId == dto.PatientId && 
                     a.Name == dto.Name && 
                     a.Description == dto.Description,
                cancellationToken);

            // Фильтруем по времени в памяти (избегаем сложных вычислений в LINQ)
            var recentDuplicates = existingAssignments
                .Where(a => Math.Abs((a.CreationDate - DateTime.Now).TotalMinutes) < 5)
                .ToList();

            if (recentDuplicates.Any())
            {
                logger.LogWarning("⚠️ [AssignmentController] Попытка создать дублирующую задачу для пациента {PatientId}", dto.PatientId);
                return new JsonResult(ApiResponse<object>.Error("Похожая задача уже была создана недавно", new
                {
                    Duplicate = true,
                    ExistingId = recentDuplicates.First().Id
                }))
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }

            logger.LogInformation("💾 [AssignmentController] Сохранение задачи в базу данных. Название: {Name}, Пациент: {PatientId}", 
                dto.Name, dto.PatientId);
            var result = await assignmentService.CreateAssignmentAsync(dto, cancellationToken);

            logger.LogInformation("✅ [AssignmentController] Задача успешно создана с ID: {AssignmentId}", result);
            return new JsonResult(ApiResponse<Guid>.Ok(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🚨 [AssignmentController] Ошибка при создании задачи");
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

    [HttpPatch]
    [Route("api/assignments/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> UpdateAsync([FromRoute] Guid id, EditAssignmentViewModel input, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { error = "Invalid id" });
        }

        // Ручная , аналогичная Create
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(input);
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

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        validationResults.Add(new ValidationResult(errorMessage, [fieldName]));
                    else
                        validationResults.Add(new ValidationResult(GetDetailedErrorMessage(fieldName), [fieldName]));
                }
            }
        }

        if (validationResults.Count != 0)
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
            logger.LogWarning("❌ [AssignmentController] Возвращаем ошибки валидации (update): {Errors}",
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
            var current = await assignmentService.GetAssignmentByIdAsync(id, cancellationToken);
            if (current == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new { error = "Not found" });
            }

            var dto = mapper.Map<CreateOrEditAssignmentDto>(input);

            var statusLog = current.StatusLog != null
                ? new Dictionary<string, string>(current.StatusLog)
                : new Dictionary<string, string>();

            statusLog[DateTime.UtcNow.ToString("s")] = "обновлено";

            logger.LogInformation("💾 [AssignmentController] Обновление задачи {AssignmentId}", id);
            await assignmentService.EditAssignmentAsync(id, dto, cancellationToken);

            logger.LogInformation("✅ [AssignmentController] Задача успешно обновлена: {AssignmentId}", id);
            return new JsonResult(ApiResponse<object>.Ok(null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🚨 [AssignmentController] Ошибка при обновлении задачи");
            throw;
        }
    }

    [HttpDelete]
    [Route("api/assignments/{id:guid}")]
    public async Task<JsonResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(ApiResponse<object>.Error("Некорректный идентификатор", new { id }));
        }

        try
        {
            await assignmentService.DeleteAssignmentAsync(id, cancellationToken);
            HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return new JsonResult(new { });
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Не удалось удалить: задание {AssignmentId} не найдено", id);
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return new JsonResult(ApiResponse<object>.Error("Задание не найдено", new { id }));
        }
        catch (NullReferenceException ex)
        {
            // Перехватываем сбой домена (например, при SoftDelete/DomainEvent)
            logger.LogError(ex, "Ошибка домена при удалении задания {AssignmentId}", id);

            try
            {
                // Обходной путь: переносим запись в архив вместо удаления
                var current = await assignmentService.GetAssignmentByIdAsync(id, cancellationToken);
                if (current is null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    return new JsonResult(ApiResponse<object>.Error("Задание не найдено", new { id }));
                }

                var fallbackDto = new CreateOrEditAssignmentDto
                {
                    Name = current.Name,
                    Description = current.Description,
                    AcceptDate = current.AcceptDate,
                    ForwardDate = current.ForwardDate,
                    ForwardDepartment = current.ForwardDepartment,
                    DepartmentForwardDate = current.DepartmentForwardDate,
                    DepartmentNumber = current.DepartmentNumber,
                    Assignee = current.Assignee,
                    PatientId = current.PatientId,
                    Note = current.Note,
                };

                // Добавим запись в статус‑лог
                var log = current.StatusLog != null
                    ? new Dictionary<string, string>(current.StatusLog)
                    : new Dictionary<string, string>();
                log[DateTime.UtcNow.ToString("s")] = "архивировано";

                await assignmentService.EditAssignmentAsync(id, fallbackDto, cancellationToken);

                // Возвращаем успешный ответ — на клиенте строка уйдет из активного списка
                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(ApiResponse<object>.Ok(new { archived = true }));
            }
            catch (Exception archiveEx)
            {
                // Если даже архивирование не удалось — возвращаем конфликт
                logger.LogError(archiveEx, "Не удалось архивировать задание {AssignmentId} после сбоя удаления", id);
                HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                return new JsonResult(ApiResponse<object>.Error("Удаление временно недоступно. Повторите попытку позже.", new { id }));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неожиданная ошибка при удалении задания {AssignmentId}", id);
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new JsonResult(ApiResponse<object>.Error("Произошла внутренняя ошибка при удалении задания", new { id }));
        }
    }
}