using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Specifications;
using CRM.SocialDepartment.Site.Helpers;
using CRM.SocialDepartment.Site.Models;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.Site.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Site.Controllers
{
    public class PatientController(
        ILogger<PatientController> logger,
        IMapper mapper,
        PatientAppService patientAppService
    ) : Controller
    {
        private readonly ILogger<PatientController> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly PatientAppService _patientAppService = patientAppService;
        private readonly DisabilityGroupWithoutPeriodSpecification _withoutPeriodSpec = new();

        // VIEW ////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Вывод всех пациентов, которые находятся в больнице (на текущий момент).
        /// </summary>
        /// <returns></returns>
        public IActionResult Active()
        {
            //todo: Вывод пациентов для медицинского персонала ограничен! Необходимо выводить только тех пациентов, которые находятся в его отделение.
            return View(nameof(Index));
        }

        /// <summary>
        /// Получить форму: Добавить пациента.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/modal/create")]
        public IActionResult GetCreatePatientModal()
        {
            ViewData.Model = new CreatePatientViewModel()
            {
                CitizenshipInfo = new ViewModels.Patient.CitizenshipInfo()
                {
                    Citizenships = ["РФ", "Иностранец", "ЛБГ"],
                    Citizenship = CitizenshipType.RussianFederation,
                    Country = "Россия",
                    NotRegistered = false,
                    
                },
                MedicalHistory = new ViewModels.Patient.MedicalHistory()
                {
                    HospitalizationType = HospitalizationType.Force,
                    NumberDepartment = 1
                },
                Documents = new Dictionary<DocumentType, DocumentViewModel>
                {
                    { DocumentType.Passport, DocumentHelper.CreateViewModel(DocumentType.Passport) },
                    { DocumentType.MedicalPolicy, DocumentHelper.CreateViewModel(DocumentType.MedicalPolicy) },
                    { DocumentType.Snils, DocumentHelper.CreateViewModel(DocumentType.Snils) }
                },
                IsCapable = true,
                ReceivesPension = false
            };

            return new PartialViewResult
            {
                ViewName = $"~/Views/Patient/_CreatePatientModal.cshtml",
                ViewData = ViewData
            };
        }

        /// <summary>
        /// Получить форму: Редактировать пациента.
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/modal/edit")]
        public async Task<IActionResult> GetEditPatientModalAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            //todo: Проверить права: Сотрудник, Администратор

            var patient = await _patientAppService.GetPatientByIdAsync(patientId, cancellationToken);

            if (patient is null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<EditPatientViewModel>(patient);

            switch (viewModel.CitizenshipInfo.Citizenship)
            {
                case 0: //Российская Федерация
                    viewModel.CountryIsEnable        = "display:none;";
                    break;

                case 1: //Иностранец
                    viewModel.NoRegistrationIsEnable = "display:none;";
                    break;

                case 2: //ЛБГ
                    viewModel.RegistrationIsEnable   = "display:none;";
                    viewModel.NoRegistrationIsEnable = "display:none;";
                    viewModel.CountryIsEnable        = "display:none;";
                    viewModel.DocumentIsEnable       = "display:none;";
                    break;
            }

            if (viewModel.IsCapable)
            {
                viewModel.CapableIsEnable            = "display:none;";
            }

            if (!viewModel.ReceivesPension)
            {
                viewModel.PensionFieldsetIsEnable    = "display:none;";
            }

            DisabilityGroup? disabilityGroup = viewModel.Pension?.DisabilityGroup;

            if (disabilityGroup is not null && !_withoutPeriodSpec.IsSatisfiedBy(disabilityGroup))
            {
                viewModel.PensionStartDateTimeIsEnable = "display:none;";
            }

            ViewData.Model = viewModel;

            return new PartialViewResult
            {
                ViewName = $"~/Views/Patient/_EditPatientModal.cshtml",
                ViewData = ViewData
            };
        }

        /// <summary>
        /// Вывод всех пациентов, которые выписались из больницы.
        /// </summary>
        /// <returns></returns>
        public IActionResult Archive()
        {
            //todo: Вывод пациентов для медицинского персонала ограничен! Запретить выводить пациентов из архива!
            return View();
        }

        //todo: Персональная страница пациента

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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddPatientsAsync(CreatePatientViewModel input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(ApiResponse<object>.Error("Неверные данные", new
                {
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                }))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var dto = _mapper.Map<CreatePatientDTO>(input);
            var result = await _patientAppService.AddPatientAsync(dto, cancellationToken);
            return new JsonResult(ApiResponse<Guid>.Ok(result));
        }

        //5. Редактировать пользователя
        [HttpPatch]
        [Route("api/patients/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditPatientsAsync([FromRoute] Guid id, EditPatientViewModel input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(ApiResponse<object>.Error("Неверные данные", new
                {
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                }))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var dto = _mapper.Map<EditPatientDTO>(input);
            await _patientAppService.EditPatientAsync(id, dto, cancellationToken);
            return new JsonResult(ApiResponse<object>.Ok(null));
        }

        //6. Удалить пользователя
        //[HttpDelete]
        //[Route("api/patients/{id:guid}")]
        //public async Task<JsonResult> DeletePatientsAsync([FromRoute] Guid id, [FromBody] CreateOrEditPatientDTO input, CancellationToken cancellationToken)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //        return new JsonResult(ModelState);
        //    }

        //    //TODO: Доделать реализацию логики
        //    await _patientAppService.DeletePatientAsync(id, cancellationToken);

        //    HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        //    return new JsonResult(new { });
        //}
    }
}
