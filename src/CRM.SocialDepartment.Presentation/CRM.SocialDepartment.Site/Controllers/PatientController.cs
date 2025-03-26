using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Site.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Site.Controllers
{
    public class PatientController(PatientAppService patientAppService) : Controller
    {
        private readonly PatientAppService _patientAppService = patientAppService;

        // VIEW ////////////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Active()
        {
            return View(nameof(Index));
        }

        public IActionResult Archive()
        {
            return View();
        }

        // API /////////////////////////////////////////////////////////////////////////////////////////////////

        //1. Получить всех пациентов
        [HttpGet]
        [Route("api/patients")]
        public async Task<IActionResult> GetAllPatientsAsync(CancellationToken cancellationToken)
        {
            var patients = await _patientAppService.GetAllPatients(null, cancellationToken);
            return Ok(patients);
        }

        //2. Получить пациента по ID
        [HttpGet]
        [Route("api/patients/{id:guid}")]
        public async Task<JsonResult> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(ModelState);
            }

            var patient = await _patientAppService.GetPatientByIdAsync(id, cancellationToken);

            if (patient is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new {});
            }

            return new JsonResult(patient);
        }

        //3. Получить всех активных (которые находятся в больнице) пациентов (для DataTablesNet)
        [HttpPost]
        [Route("api/patients/active")]
        public async Task<JsonResult> GetPatientActiveForDataTableNetAsync([FromServices] DataTableNetService dataTableNetService, CancellationToken cancellationToken = default)
        {
            //todo: Добавить логгирование уровня Debug

            var input = dataTableNetService.Parse(Request);

            var filter = Builders<Patient>.Filter.Or(
                Builders<Patient>.Filter.Where(i => !i.SoftDeleted), // Фильтр для мягкого удаления
                Builders<Patient>.Filter.Where(i => !i.IsArchive));  // Фильтр для архива

            // Поиск по ключевому слову
            if (!string.IsNullOrEmpty(input.SearchTerm))
            {
                var searchTemp = input.SearchTerm.ToLower();

                filter = Builders<Patient>.Filter.Or(
                    Builders<Patient>.Filter.Where(i => i.FullName.Contains(searchTemp, StringComparison.CurrentCultureIgnoreCase))//,
                    //Builders<Patient>.Filter.Where(i => i.Registration != null && i.Registration.ToLower().Contains(searchTemp)),
                    //Builders<Patient>.Filter.Where(i => i.NumberDepartment.ToString().Contains(searchTemp))
                );
            }

            // Получить общее количество записей до поиска
            var totalRecords = await _patientAppService.GetPatientCollection().CountDocumentsAsync(Builders<Patient>.Filter.Empty, cancellationToken: cancellationToken);

            // Получить общее количество записей после поиска
            var filteredRecords = await _patientAppService.GetPatientCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            // Пагинация
            var patients = await _patientAppService.GetPatientCollection().Find(filter)
                .Skip(input.Skip)
                .Limit(input.PageSize)
                .ToListAsync(cancellationToken);

            // Преобразовать данные для представления
            var result = patients.Select(i =>
            {
                //var historyOfIllness = i.HistoryOfIllnesses.FirstOrDefault(h => h.IsActive);

                return new //TODO: Сделать DTO
                {
                    i.Id,
                    //HospitalizationType = historyOfIllness?.HospitalizationType.GetAttribute<DisplayAttribute>()?.Name,
                    //historyOfIllness?.Resolution,
                    //historyOfIllness?.NumberDocument,
                    //DateOfReceipt = historyOfIllness?.DateOfReceipt.ToString("dd.MM.yyyy"),
                    //i.NumberDepartment,
                    i.FullName,
                    //Birthday = i.Birthday.ToString("dd.MM.yyyy"),
                    //IsChildren = i.IsChildren ? "Несовершеннолетний" : "Совершеннолетний",
                    //i.Citizenship,
                    //i.Country,
                    //Registration = i.Registration ?? "",
                    //NoRegistration = i.NoRegistration ? "БОМЖ" : "Нет",
                    //EarlyRegistration = i.EarlyRegistration?.GetAttribute<DisplayAttribute>()?.Name,
                    //i.PlaceOfBirth,
                    //IsCapable = i.IsCapable ? "Дееспособный" : "Недееспособный",
                    //PensionIsActive = i.PensionIsActive ? "Да" : "Нет",
                    //DisabilityGroup = i.DisabilityGroup?.GetAttribute<DisplayAttribute>()?.Name,
                    //Note = i.Note ?? ""
                };
            });

            return new JsonResult(new
            {
                draw = input.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = result
            });
        }

        //4. Получить всех пациентов, которые находятся в архиве (выписаны из больницы) (для DataTablesNet)
        [HttpPost]
        [Route("api/patients/archive")]
        public async Task<JsonResult> GetPatientArchiveForDataTableNetAsync([FromServices] DataTableNetService dataTableNetService, CancellationToken cancellationToken = default)
        {
            //todo: Добавить логгирование уровня Debug

            var input = dataTableNetService.Parse(Request);

            var filter = Builders<Patient>.Filter.Or(
                Builders<Patient>.Filter.Where(i => !i.SoftDeleted), // Фильтр для мягкого удаления
                Builders<Patient>.Filter.Where(i => i.IsArchive));   // Фильтр для архива

            // Поиск по ключевому слову
            if (!string.IsNullOrEmpty(input.SearchTerm))
            {
                var searchTemp = input.SearchTerm.ToLower();

                filter = Builders<Patient>.Filter.Or(
                    Builders<Patient>.Filter.Where(i => i.FullName.Contains(searchTemp, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            // Получить общее количество записей до поиска
            var totalRecords = await _patientAppService.GetPatientCollection().CountDocumentsAsync(Builders<Patient>.Filter.Empty);

            // Получить общее количество записей после поиска
            var filteredRecords = await _patientAppService.GetPatientCollection().CountDocumentsAsync(filter);

            // Пагинация
            var patients = await _patientAppService.GetPatientCollection().Find(filter)
                .Skip(input.Skip)
                .Limit(input.PageSize)
                .ToListAsync();

            // Преобразовать данные для представления
            var result = patients.Select(i =>
            {
                return new //TODO: Сделать DTO
                {
                    i.Id,
                    //Номер истории болезни
                    //Тип госпитализации
                    i.FullName,
                    //Постановление
                    //Дата поступления
                    //Дата выписки
                    //Примечание
                };
            });

            return new JsonResult(new
            {
                draw = input.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = result
            });
        }

        //4. Добавить пациента
        [HttpPost]
        [Route("api/patients")]
        public async Task<JsonResult> AddPatientsAsync([FromBody] CreateOrEditPatientDTO input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(ModelState);
            }

            var result = await _patientAppService.AddPatientAsync(input, cancellationToken);
            return new JsonResult(result);
        }

        //5. Редактировать пользователя
        [HttpPatch]
        [Route("api/patients/{id:guid}")]
        public async Task<JsonResult> EditPatientsAsync([FromRoute] Guid id, [FromBody] CreateOrEditPatientDTO input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(ModelState);
            }

            await _patientAppService.EditPatientAsync(id, input, cancellationToken);

            HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return new JsonResult(new {});
        }

        //6. Удалить пользователя
        [HttpDelete]
        [Route("api/patients/{id:guid}")]
        public async Task<JsonResult> DeletePatientsAsync([FromRoute] Guid id, [FromBody] CreateOrEditPatientDTO input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(ModelState);
            }

            //TODO: Доделать реализацию логики
            await _patientAppService.DeletePatientAsync(id, cancellationToken);

            HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return new JsonResult(new { });
        }
    }
}
