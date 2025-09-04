using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Specifications;
using CRM.SocialDepartment.Site.Filters;
using CRM.SocialDepartment.Site.Helpers;
using CRM.SocialDepartment.Site.Models;
using CRM.SocialDepartment.Site.Services;
using CRM.SocialDepartment.Site.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations;

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
        [LogDataRequest("Просмотр активных пациентов", "Пользователь запросил список активных пациентов")]
        public IActionResult Active()
        {
            //todo: Вывод пациентов для медицинского персонала ограничен! Необходимо выводить только тех пациентов, которые находятся в его отделение.
            return View(nameof(Index));
        }

        /// <summary>
        /// Отображение карточки пациента.s
        /// </summary>
        /// <param name="id">ID пациента</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/Card/{id:guid}")]
        [LogDataRequest("Просмотр карточки пациента", "Пользователь запросил карточку пациента")]
        public async Task<IActionResult> Card(Guid id, CancellationToken cancellationToken = default)
        {
            var patientCard = await _patientAppService.GetPatientCardAsync(id, cancellationToken);
            
            if (patientCard == null)
            {
                return NotFound();
            }

            return View(patientCard);
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
                CitizenshipInfo = new Models.Patient.CitizenshipInfoModel()
                {
                    Citizenships = ["РФ", "Иностранец", "ЛБГ"],
                    Citizenship = CitizenshipType.RussianFederation,
                    Country = "Россия",
                    NotRegistered = false
                },
                MedicalHistory = new Models.Patient.MedicalHistoryModel()
                {
                    HospitalizationType = HospitalizationType.Force,
                    NumberDepartment = 1,
                    DateOfReceipt = DateTime.Today,
                },
                Documents = new Dictionary<DocumentType, DocumentViewModel>
                {
                    { DocumentType.Passport, DocumentHelper.CreateViewModel(DocumentType.Passport) },
                    { DocumentType.MedicalPolicy, DocumentHelper.CreateViewModel(DocumentType.MedicalPolicy) },
                    { DocumentType.Snils, DocumentHelper.CreateViewModel(DocumentType.Snils) }
                },
                IsCapable = true, // По умолчанию дееспособен
                Capable = null,
                ReceivesPension = false, // По умолчанию не получает пенсию
                Pension = null
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
        [Route("[controller]/modal/edit/{patientId:guid}")]
        public async Task<IActionResult> GetEditPatientModalAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            //todo: Проверить права: Сотрудник, Администратор

            var patient = await _patientAppService.GetPatientByIdAsync(patientId, cancellationToken);

            if (patient is null)
            {
                return NotFound();
            }

            // Логируем данные пациента для диагностики
            _logger.LogInformation("🔍 [PatientController] Данные пациента для редактирования:");
            _logger.LogInformation("👤 [PatientController] Пациент: {FullName}", patient.FullName);
            _logger.LogInformation("📅 [PatientController] ActiveHistory: {ActiveHistory}", patient.ActiveHistory != null ? "Есть" : "Нет");
            if (patient.ActiveHistory != null)
            {
                _logger.LogInformation("📅 [PatientController] DateOfReceipt: {DateOfReceipt}", patient.ActiveHistory.DateOfReceipt);
                _logger.LogInformation("📅 [PatientController] NumberDepartment: {NumberDepartment}", patient.ActiveHistory.NumberDepartment);
                _logger.LogInformation("📅 [PatientController] Resolution: {Resolution}", patient.ActiveHistory.Resolution);
            }

            var viewModel = _mapper.Map<EditPatientViewModel>(patient);
            
            // Логируем данные ViewModel после маппинга
            _logger.LogInformation("🔍 [PatientController] Данные ViewModel после маппинга:");
            _logger.LogInformation("📅 [PatientController] ViewModel DateOfReceipt: {DateOfReceipt}", viewModel.MedicalHistory.DateOfReceipt);
            _logger.LogInformation("📅 [PatientController] ViewModel DateOfReceipt.Date: {DateOfReceiptDate}", viewModel.MedicalHistory.DateOfReceipt.Date);
            _logger.LogInformation("📅 [PatientController] ViewModel DateOfReceipt.ToString('yyyy-MM-dd'): {DateOfReceiptFormatted}", viewModel.MedicalHistory.DateOfReceipt.ToString("yyyy-MM-dd"));
            _logger.LogInformation("📅 [PatientController] ViewModel NumberDepartment: {NumberDepartment}", viewModel.MedicalHistory.NumberDepartment);

            switch (viewModel.CitizenshipInfo.Citizenship)
            {
                case 0: //Российская Федерация
                    viewModel.CountryIsEnable        = "display:none;";
                    viewModel.DocumentIsEnable       = ""; // Показываем документы
                    break;

                case 1: //Иностранец
                    viewModel.NoRegistrationIsEnable = "display:none;";
                    viewModel.DocumentIsEnable       = ""; // Показываем документы
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

            DisabilityGroupType? disabilityGroup = viewModel.Pension?.DisabilityGroup;

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
            var patients = await _patientAppService.GetAllPatientsAsync(null, cancellationToken);
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

        //3. Получить всех активных пациентов (для DataTablesNet)
        [HttpPost]
        [Route("api/patients/active")]
        public async Task<JsonResult> GetPatientActiveForDataTableNetAsync([FromServices] DataTableNetService dataTableNetService, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("🔍 Получен запрос на /api/patients/active");
                var input = dataTableNetService.Parse(Request);
                _logger.LogInformation("📋 Параметры запроса: {@Input}", input);

                // Проверяем входные параметры
                if (input == null)
                {
                    _logger.LogWarning("⚠️ Получен пустой input от DataTableNetService");
                    return Json(new
                    {
                        draw = "1",
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>()
                    });
                }

                // Преобразуем в доменные параметры
                var parameters = new DataTableParameters
                {
                    Skip = input.Skip,
                    PageSize = input.PageSize,
                    SearchTerm = input.SearchTerm
                };

                // Используем доменный метод репозитория с AutoMapper
                var result = await _patientAppService.GetActivePatientsForDataTableAsync(parameters, cancellationToken);

                // Преобразовать данные для представления (полный набор полей, ожидаемых таблицей)
                var dataResult = result.Data.Select(x => new
                {
                    id = x.Id,
                    hospitalizationType = x.HospitalizationType,
                    resolution = x.CourtDecision,
                    medicalHistoryNumber = x.NumberDocument,
                    dateOfReceipt = x.DateOfReceipt != DateTime.MinValue ? x.DateOfReceipt.ToString("dd.MM.yyyy") : null,
                    department = x.Department,
                    fullName = x.FullName,
                    birthday = x.Birthday.ToString("dd.MM.yyyy"),
                    isChildren = x.IsChildren,
                    citizenship = x.Citizenship,
                    country = x.Country,
                    registration = x.Registration,
                    notRegistered = x.IsHomeless,
                    earlyRegistration = x.EarlyRegistration,
                    placeOfBirth = x.PlaceOfBirth,
                    IsCapable = x.IsCapable,
                    ReceivesPension = x.ReceivesPension,
                    DisabilityGroup = x.DisabilityGroup,
                    Note = x.Note
                });

                _logger.LogInformation("✅ Возвращаем данные: {TotalCount} записей", result.TotalRecords);
                return Json(new
                {
                    draw = input.Draw,
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.FilteredRecords,
                    data = dataResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных пациентов для DataTable");
                
                // Возвращаем пустой результат вместо ошибки
                return Json(new
                {
                    draw = "1",
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = "Произошла ошибка при загрузке данных"
                });
            }
        }

        //4. Получить всех пациентов, которые находятся в архиве (выписаны из больницы) (для DataTablesNet)
        [HttpPost]
        [Route("api/patients/archive")]
        public async Task<JsonResult> GetPatientArchiveForDataTableNetAsync([FromServices] DataTableNetService dataTableNetService, CancellationToken cancellationToken = default)
        {
            try
            {
                //todo: Добавить логгирование уровня Debug

                var input = dataTableNetService.Parse(Request);

                // Преобразуем в доменные параметры
                var parameters = new DataTableParameters
                {
                    Skip = input.Skip,
                    PageSize = input.PageSize,
                    SearchTerm = input.SearchTerm
                };

                // Используем доменный метод репозитория
                var result = await _patientAppService.GetArchivedPatientsForDataTableAsync(parameters, cancellationToken);

                // Преобразовать данные для представления
                var dataResult = result.Data.Select(i =>
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
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.FilteredRecords,
                    data = dataResult
                });
            }
            catch (MongoDB.Driver.MongoCommandException ex) when (ex.Message.Contains("Regular expression is invalid"))
            {
                Console.WriteLine($"🚨 [PatientController] Ошибка регулярного выражения в поиске архивных пациентов: {ex.Message}");
                
                // Возвращаем ошибку с понятным сообщением для пользователя
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(new
                {
                    error = "Некорректный символ в поиске",
                    message = "Введенные символы не могут быть использованы для поиска. Попробуйте изменить поисковый запрос.",
                    details = "Специальные символы вроде /, \\, *, +, ?, ^, $, |, (, ), [, ], {, } могут вызывать ошибки поиска."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 [PatientController] Общая ошибка при получении архивных пациентов: {ex.Message}");
                
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(new
                {
                    error = "Ошибка сервера",
                    message = "Произошла внутренняя ошибка при получении данных. Обратитесь к администратору."
                });
            }
        }

        //4. Добавить пациента
        [HttpPost]
        [Route("api/patients")]
        [ValidateAntiForgeryToken]
        [LogCreate("Patient", "Создание пациента", "Пользователь создал нового пациента")]
        public async Task<JsonResult> AddPatientsAsync(CreatePatientViewModel input, CancellationToken cancellationToken)
        {
            // Очищаем ModelState от автоматических ошибок для вложенных моделей
            // и используем только ручную валидацию
            var modelStateErrorsToRemove = new List<string>();
            
            foreach (var modelError in ModelState)
            {
                var fieldName = modelError.Key;
                
                // Удаляем автоматические ошибки для вложенных моделей
                if (fieldName.StartsWith("MedicalHistory.") ||
                    fieldName.StartsWith("CitizenshipInfo.") ||
                    fieldName.StartsWith("Capable.") ||
                    fieldName.StartsWith("Pension.") ||
                    fieldName.StartsWith("Documents"))
                {
                    modelStateErrorsToRemove.Add(fieldName);
                }
            }
            
            // Удаляем найденные ошибки из ModelState
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
            
            // Проверяем базовые поля (FullName, Birthday) и улучшаем сообщения об ошибках
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        var fieldName = modelError.Key;
                        var errorMessage = error.ErrorMessage;
                        
                        // Обрабатываем только базовые поля
                        if (fieldName == "FullName" || fieldName == "Birthday")
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
            
            // Если есть ошибки валидации, возвращаем их
            if (validationResults.Any())
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                
                _logger.LogWarning("❌ [PatientController] Возвращаем ошибки валидации: {Errors}", string.Join(", ", errors));
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
                _logger.LogInformation("📄 [PatientController] Documents count: {Count}", input.Documents?.Count ?? 0);
                if (input.Documents != null)
                {
                    foreach (var doc in input.Documents)
                    {
                        _logger.LogInformation("📄 [PatientController] Документ: {Type} = {Number}", doc.Key.DisplayName, doc.Value.Number);
                    }
                }
                
                var dto = _mapper.Map<CreatePatientDTO>(input);
                _logger.LogInformation("📄 [PatientController] DTO Documents count: {Count}", dto.Documents?.Count ?? 0);
                
                _logger.LogInformation("💾 [PatientController] Сохранение пациента в базу данных");
                var result = await _patientAppService.AddPatientAsync(dto, cancellationToken);
                
                _logger.LogInformation("✅ [PatientController] Пациент успешно создан с ID: {PatientId}", result);
                return new JsonResult(ApiResponse<Guid>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 [PatientController] Ошибка при создании пациента");
                throw;
            }
        }
        
        /// <summary>
        /// Извлекает индекс документа из имени поля
        /// </summary>
        private static int ExtractDocumentIndex(string fieldName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(fieldName, @"Documents\[(\d+)\]");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }
        
        /// <summary>
        /// Получает название документа по индексу
        /// </summary>
        private static string GetDocumentNameByIndex(int index)
        {
            return index switch
            {
                0 => "Паспорт",
                1 => "Полис ОМС", 
                2 => "СНИЛС",
                _ => "Документ"
            };
        }
        
        /// <summary>
        /// Создает детальное сообщение об ошибке на основе имени поля
        /// </summary>
        private static string GetDetailedErrorMessage(string fieldName)
        {
            return fieldName switch
            {
                "FullName" => "ФИО содержит недопустимые символы или слишком короткое",
                "Birthday" => "Дата рождения указана некорректно",
                "MedicalHistory.HospitalizationType" => "Некорректный тип госпитализации",
                "MedicalHistory.NumberDocument" => "Некорректный номер медицинского документа",
                "MedicalHistory.DateOfReceipt" => "Некорректная дата получения документа",
                "MedicalHistory.NumberDepartment" => "Номер отделения должен быть от 1 до 30",
                "CitizenshipInfo.Citizenship" => "Некорректный тип гражданства",
                "CitizenshipInfo.Country" => "Некорректное название страны",
                "CitizenshipInfo.Registration" => "Некорректный адрес регистрации",
                _ when fieldName.Contains("Documents") && fieldName.Contains("Number") => 
                    $"Некорректный формат номера документа: {GetDocumentNameFromFieldName(fieldName)}",
                _ when fieldName.Contains("Capable") => "Некорректные данные о дееспособности",
                _ when fieldName.Contains("Pension") => "Некорректные данные о пенсии",
                "Note" => "Некорректное примечание",
                _ => $"Введено некорректное значение в поле '{fieldName}'"
            };
        }
        
        /// <summary>
        /// Получает название документа из имени поля
        /// </summary>
        private static string GetDocumentNameFromFieldName(string fieldName)
        {
            if (fieldName.Contains("Documents[0]")) return "Паспорт";
            if (fieldName.Contains("Documents[1]")) return "Полис ОМС";
            if (fieldName.Contains("Documents[2]")) return "СНИЛС";
            return "Документ";
        }

        //5. Редактировать пользователя
        [HttpPut]
        [Route("api/patients/{id:guid}")]
        [ValidateAntiForgeryToken]
        [LogUpdate("Patient", "Редактирование пациента", "Пользователь отредактировал данные пациента")]
        public async Task<JsonResult> EditPatientAsync([FromRoute] Guid id, EditPatientViewModel input, CancellationToken cancellationToken)
        {
            // Исключаем поля с [BindNever] из валидации
            var fieldsToExclude = new[]
            {
                nameof(EditPatientViewModel.NoRegistrationIsEnable),
                nameof(EditPatientViewModel.CountryIsEnable),
                nameof(EditPatientViewModel.RegistrationIsEnable),
                nameof(EditPatientViewModel.EarlyRegistrationIsEnable),
                nameof(EditPatientViewModel.LbgIsEnable),
                nameof(EditPatientViewModel.DocumentIsEnable),
                nameof(EditPatientViewModel.CapableIsEnable),
                nameof(EditPatientViewModel.PensionFieldsetIsEnable),
                nameof(EditPatientViewModel.PensionStartDateTimeIsEnable)
            };

            foreach (var field in fieldsToExclude)
            {
                ModelState.Remove(field);
            }

            // Условная валидация для Capable - только если IsCapable = false
            if (input.IsCapable && input.Capable != null)
            {
                ModelState.Remove("Capable.CourtDecision");
                ModelState.Remove("Capable.TrialDate");
                ModelState.Remove("Capable.Guardian");
                ModelState.Remove("Capable.GuardianOrderAppointment");
            }

            // Условная валидация для Pension - только если ReceivesPension = true
            if (!input.ReceivesPension && input.Pension != null)
            {
                ModelState.Remove("Pension.DisabilityGroup");
                ModelState.Remove("Pension.PensionStartDateTime");
                ModelState.Remove("Pension.PensionAddress");
                ModelState.Remove("Pension.SfrBranch");
                ModelState.Remove("Pension.SfrDepartment");
                ModelState.Remove("Pension.Rsd");
            }

            // Удаляем ошибки валидации для пустых документов
            foreach (var document in input.Documents)
            {
                if (string.IsNullOrWhiteSpace(document.Value.Number))
                {
                    ModelState.Remove($"Documents[{document.Key}].Number");
                    ModelState.Remove($"Documents[{document.Key}]");
                }
            }
            
            // Удаляем общие ошибки валидации для Documents если все документы пустые
            if (input.Documents.All(d => string.IsNullOrWhiteSpace(d.Value.Number)))
            {
                ModelState.Remove("Documents");
            }

            // Удаляем ошибки валидации для DocumentAttached если он пустой
            if (string.IsNullOrWhiteSpace(input.CitizenshipInfo.DocumentAttached))
            {
                ModelState.Remove("CitizenshipInfo.DocumentAttached");
            }

            // Удаляем ошибки валидации для даты поступления если она не задана
            if (input.MedicalHistory.DateOfReceipt == DateTime.MinValue || input.MedicalHistory.DateOfReceipt == default(DateTime))
            {
                ModelState.Remove("MedicalHistory.DateOfReceipt");
            }

            // Логируем данные для диагностики
            _logger.LogInformation("🔍 [PatientController] Данные для редактирования:");
            _logger.LogInformation("📅 [PatientController] DateOfReceipt: {DateOfReceipt}", input.MedicalHistory.DateOfReceipt);
            _logger.LogInformation("📄 [PatientController] Documents count: {Count}", input.Documents?.Count ?? 0);
            if (input.Documents != null)
            {
                foreach (var doc in input.Documents)
                {
                    _logger.LogInformation("📄 [PatientController] Документ {Type}: '{Number}'", doc.Key.DisplayName, doc.Value.Number);
                }
            }

            // Логируем все ошибки валидации для диагностики
            _logger.LogInformation("🔍 [PatientController] Проверяем ModelState после очистки...");
            foreach (var modelState in ModelState)
            {
                if (modelState.Value.Errors.Any())
                {
                    _logger.LogWarning("⚠️ [PatientController] Поле '{FieldName}' имеет ошибки: {Errors}", 
                        modelState.Key, 
                        string.Join(", ", modelState.Value.Errors.Select(e => e.ErrorMessage ?? "Пустое сообщение")));
                }
            }

            if (!ModelState.IsValid)
            {
                // Собираем подробные ошибки валидации
                var detailedErrors = new List<string>();
                foreach (var modelState in ModelState)
                {
                    var fieldName = modelState.Key;
                    var errors = modelState.Value.Errors;
                    
                    foreach (var error in errors)
                    {
                        var errorMessage = string.IsNullOrEmpty(error.ErrorMessage) 
                            ? $"Поле '{fieldName}' имеет недопустимое значение" 
                            : error.ErrorMessage;
                        detailedErrors.Add($"{fieldName}: {errorMessage}");
                        _logger.LogWarning("❌ [PatientController] Ошибка валидации: {FieldName} - {ErrorMessage}", fieldName, errorMessage);
                    }
                }

                _logger.LogWarning("❌ [PatientController] Возвращаем ошибки валидации: {Errors}", string.Join(", ", detailedErrors));
                return new JsonResult(ApiResponse<object>.Error("Неверные данные", new
                {
                    Errors = detailedErrors
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
        [HttpDelete]
        [Route("api/patients/{id:guid}")]
        public async Task<JsonResult> DeletePatientsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
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
