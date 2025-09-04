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
            DataTableNetModel input = null;

            try
            {
                _logger.LogInformation("🔍 Получен запрос на /api/patients/active");
                input = dataTableNetService.Parse(Request);
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

                var patients = await _patientAppService.GetAllPatientsAsync(null, cancellationToken);
                
                // Проверяем, что пациенты не null
                if (patients == null)
                {
                    _logger.LogWarning("⚠️ Получен null список пациентов");
                    return Json(new
                    {
                        draw = input.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>()
                    });
                }
                
                var query = patients.AsQueryable();

                // Проверяем, есть ли пациенты
                if (!patients.Any())
                {
                    _logger.LogInformation("📭 Нет активных пациентов в базе данных");
                    return Json(new
                    {
                        draw = input.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>()
                    });
                }

                // Поиск
                if (!string.IsNullOrEmpty(input.SearchTerm))
                {
                    _logger.LogInformation("🔍 Выполняем поиск по термину: '{SearchTerm}'", input.SearchTerm);
                    
                    // Фильтруем пациентов с null свойствами перед поиском
                    var validPatients = patients.Where(p => p != null).ToList();
                    _logger.LogInformation("📋 Найдено {ValidCount} валидных пациентов", validPatients.Count);
                    
                    if (!validPatients.Any())
                    {
                        _logger.LogInformation("📭 Нет валидных пациентов для поиска");
                        return Json(new
                        {
                            draw = input.Draw,
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = new List<object>()
                        });
                    }
                    
                    query = validPatients.AsQueryable().Where(p =>
                        (p.FullName != null && p.FullName.Contains(input.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.CitizenshipInfo != null && p.CitizenshipInfo.Citizenship != null && p.CitizenshipInfo.Citizenship.ToString().Contains(input.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (p.ActiveHistory != null && p.ActiveHistory.HospitalizationType != null && p.ActiveHistory.HospitalizationType.ToString().Contains(input.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else
                {
                    _logger.LogInformation("🔍 Поисковый термин пустой, возвращаем все записи");
                    // Фильтруем пациентов с null свойствами
                    var validPatients = patients.Where(p => p != null).ToList();
                    if (!validPatients.Any())
                    {
                        _logger.LogInformation("📭 Нет валидных пациентов");
                        return Json(new
                        {
                            draw = input.Draw,
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = new List<object>()
                        });
                    }
                    query = validPatients.AsQueryable();
                }

                // Сортировка
                if (!string.IsNullOrEmpty(input.SortColumn))
                {
                    switch (input.SortColumn.ToLower())
                    {
                        case "fullname":
                            query = input.SortColumnDirection == "asc" ? query.OrderBy(p => p.FullName ?? "") : query.OrderByDescending(p => p.FullName ?? "");
                            break;
                        case "birthday":
                            query = input.SortColumnDirection == "asc" ? query.OrderBy(p => p.Birthday) : query.OrderByDescending(p => p.Birthday);
                            break;
                        case "citizenship":
                            query = input.SortColumnDirection == "asc" ? query.OrderBy(p => p.CitizenshipInfo != null ? p.CitizenshipInfo.Citizenship.ToString() : "") : query.OrderByDescending(p => p.CitizenshipInfo != null ? p.CitizenshipInfo.Citizenship.ToString() : "");
                            break;
                        default:
                            query = query.OrderBy(p => p.FullName ?? "");
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(p => p.FullName ?? "");
                }

                // Проверяем, что query не null
                if (query == null)
                {
                    _logger.LogWarning("⚠️ Query равен null, возвращаем пустой результат");
                    return Json(new
                    {
                        draw = input.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>()
                    });
                }

                _logger.LogInformation("🔍 Выполняем подсчет записей...");
                
                int totalCount;
                List<Patient> queryList;
                try
                {
                    // Проверяем, есть ли данные в query
                    queryList = query.ToList();
                    totalCount = queryList.Count;
                    _logger.LogInformation("✅ Подсчет записей завершен: {TotalCount}", totalCount);
                    
                    // Если нет данных, возвращаем пустой результат
                    if (totalCount == 0)
                    {
                        _logger.LogInformation("📭 Нет данных для отображения");
                        return Json(new
                        {
                            draw = input.Draw,
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = new List<object>()
                        });
                    }
                }
                catch (Exception countEx)
                {
                    _logger.LogError(countEx, "❌ Ошибка при подсчете записей");
                    totalCount = 0;
                    queryList = new List<Patient>();
                }

                _logger.LogInformation("📄 Выполняем пагинацию...");
                List<Patient> pagedData;
                try
                {
                    // Используем уже полученный список для пагинации
                    pagedData = queryList.Skip(input.Skip).Take(input.PageSize).ToList();
                    _logger.LogInformation("✅ Пагинация завершена: {PagedCount} записей", pagedData.Count);
                }
                catch (Exception pagingEx)
                {
                    _logger.LogError(pagingEx, "❌ Ошибка при выполнении пагинации");
                    pagedData = new List<Patient>();
                }

                _logger.LogInformation("📊 Результаты поиска: Всего записей: {TotalCount}, Показано: {PagedCount}", totalCount, pagedData.Count);

                // Проверяем, есть ли данные
                if (pagedData == null || !pagedData.Any())
                {
                    _logger.LogInformation("📭 Поиск не дал результатов, возвращаем пустой список");
                    return Json(new
                    {
                        draw = input.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>()
                    });
                }

                _logger.LogInformation("🔧 Создаем данные для ответа...");
                List<object> data;
                try
                {
                    data = pagedData.Select(p => new
                    {
                        id = p.Id,
                        hospitalizationType = p.ActiveHistory?.HospitalizationType.ToString() ?? "",
                        resolution = p.Capable?.CourtDecision ?? "",
                        medicalHistoryNumber = p.ActiveHistory?.NumberDocument ?? "",
                        dateOfReceipt = p.ActiveHistory?.DateOfReceipt.ToString("dd.MM.yyyy"),
                        department = p.ActiveHistory?.NumberDepartment.ToString() ?? "",
                        fullName = p.FullName,
                        birthday = p.Birthday.ToString("dd.MM.yyyy"),
                        isChildren = p.IsChildren,
                        citizenship = p.CitizenshipInfo?.Citizenship.ToString() ?? "",
                        country = p.CitizenshipInfo?.Country ?? "",
                        registration = p.CitizenshipInfo?.Registration ?? "",
                        notRegistered = p.CitizenshipInfo?.NotRegistered ?? false,
                        earlyRegistration = p.CitizenshipInfo?.EarlyRegistration?.DisplayName ?? "",
                        placeOfBirth = p.CitizenshipInfo?.PlaceOfBirth ?? "",
                        IsCapable = p.IsCapable,
                        ReceivesPension = p.ReceivesPension,
                        DisabilityGroup = p.Pension?.DisabilityGroup?.ToString() ?? "",
                        Note = p.Note ?? ""
                    }).Cast<object>().ToList();

                    _logger.LogInformation("✅ Данные успешно созданы: {DataCount} записей", data.Count);
                }
                catch (Exception dataEx)
                {
                    _logger.LogError(dataEx, "❌ Ошибка при создании данных");
                    data = new List<object>();
                }

                var result = new
                {
                    draw = input.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = data
                };
                
                _logger.LogInformation("✅ Возвращаем данные: {TotalCount} записей", totalCount);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных пациентов для DataTable");
                
                // Возвращаем пустой результат вместо ошибки
                return Json(new
                {
                    draw = input?.Draw ?? "1",
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
                var dto = _mapper.Map<CreatePatientDTO>(input);
                
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
        [HttpPost]
        [Route("api/patients/{id:guid}")]
        [ValidateAntiForgeryToken]
        [LogUpdate("Patient", "Редактирование пациента", "Пользователь отредактировал данные пациента")]
        public async Task<JsonResult> EditPatientAsync([FromRoute] Guid id, EditPatientViewModel input, CancellationToken cancellationToken)
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
