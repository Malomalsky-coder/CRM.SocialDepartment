using AutoMapper;
using CRM.SocialDepartment.Application.Common;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Entities.Patients.Factories;
using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Domain.Repositories;
using DDD.Events;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Application.Patients
{
    public class PatientAppService(IMapper mapper, IUnitOfWork unitOfWork, IDomainEventDispatcher? domainEventDispatcher = null)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IDomainEventDispatcher? _domainEventDispatcher = domainEventDispatcher;

        public async Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default) //TODO: ??? return PatientDTO
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            return await _unitOfWork.Patients.GetAsync((item) => item.Id == id, cancellationToken);
        }

        public async Task<PatientCardViewModel?> GetPatientCardAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await GetPatientByIdAsync(id, cancellationToken);
            if (patient == null)
                return null;

            var viewModel = new PatientCardViewModel
            {
                PatientId = patient.Id,
                FullName = patient.FullName,
                Birthday = patient.Birthday,
                Citizenship = patient.CitizenshipInfo.Citizenship.ToString(),
                Country = patient.CitizenshipInfo.Country ?? string.Empty,
                NumberDepartment = patient.ActiveHistory?.NumberDepartment.ToString() ?? string.Empty,
                Note = patient.Note,
                NoRegistration = patient.CitizenshipInfo.NotRegistered,
                EarlyRegistration = patient.CitizenshipInfo.EarlyRegistration?.Value ?? 0,
                Registration = patient.CitizenshipInfo.Registration,
                DocumentAttached = patient.CitizenshipInfo.DocumentAttached,
                Passport = patient.Documents.TryGetValue(DocumentType.FromValue(0), out var passport) ? GetDocumentNumber(passport as PassportDocument) : null,
                Snils = patient.Documents.TryGetValue(DocumentType.FromValue(2), out var snils) ? GetDocumentNumber(snils as SnilsDocument) : null,
                MedicalPolicy = patient.Documents.TryGetValue(DocumentType.FromValue(1), out var medicalPolicy) ? GetDocumentNumber(medicalPolicy as MedicalPolicyDocument) : null,
                IsCapable = patient.IsCapable,
                CourtDecision = patient.Capable?.CourtDecision,
                TrialDate = patient.Capable?.TrialDate,
                Guardian = patient.Capable?.Guardian,
                GuardianOrderAppointment = patient.Capable?.GuardianOrderAppointment,
                PensionIsActive = patient.ReceivesPension,
                DisabilityGroup = patient.Pension?.DisabilityGroup.Value ?? 0,
                PensionStartDateTime = patient.Pension?.PensionStartDateTime,
                PensionAddress = patient.Pension?.PensionAddress.Value ?? 0,
                SfrBranch = patient.Pension?.SfrBranch.ToString(),
                SfrDepartment = patient.Pension?.SfrDepartment,
                Rsd = patient.Pension?.Rsd,
                IsArchive = patient.IsArchive,
                Archive = patient.IsArchive ? new PatientArchiveViewModel
                {
                    Status = 0, // Нужно добавить свойство Status в Patient
                    Note = null // Нужно добавить свойство Note в Patient
                } : null,
                HistoryOfIllnesses = patient.MedicalHistories.Select(mh => new MedicalHistoryViewModel
                {
                    Id = mh.Id,
                    NumberDocument = mh.NumberDocument,
                    DateOfReceipt = mh.DateOfReceipt,
                    DateOfDischarge = mh.DateOfDischarge,
                    Note = mh.Note
                }).ToList()
            };

            return viewModel;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(Expression<Func<Patient, bool>>? predicate, CancellationToken cancellationToken = default)
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            return await _unitOfWork.Patients.GetAllAsync(predicate, cancellationToken);
        }

        public async Task<Guid> AddPatientAsync(CreatePatientDTO input, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"🔄 [PatientAppService] Начало создания пациента: {input.FullName}");
            Console.WriteLine($"🏥 [PatientAppService] История болезни: HospitalizationType={input.MedicalHistory.HospitalizationType.DisplayName}, NumberDepartment={input.MedicalHistory.NumberDepartment}, Resolution={input.MedicalHistory.Resolution}, NumberDocument={input.MedicalHistory.NumberDocument}, DateOfReceipt={input.MedicalHistory.DateOfReceipt}");
            Console.WriteLine($"🏛️ [PatientAppService] Гражданство: Citizenship={input.CitizenshipInfo.Citizenship}, Country={input.CitizenshipInfo.Country}, Registration={input.CitizenshipInfo.Registration}");
            Console.WriteLine($"🧠 [PatientAppService] Дееспособность: IsCapable={input.IsCapable}, Capable={input.Capable != null}");
            Console.WriteLine($"💰 [PatientAppService] Пенсия: ReceivesPension={input.ReceivesPension}, Pension={input.Pension != null}");

            HospitalizationType hospitalizationType = new(input.MedicalHistory.HospitalizationType.Value, input.MedicalHistory.HospitalizationType.DisplayName);

            MedicalHistory medicalHistory =
                new(
                    Guid.NewGuid(),
                    input.MedicalHistory.NumberDepartment,
                    hospitalizationType,
                    input.MedicalHistory.Resolution,
                    input.MedicalHistory.NumberDocument,
                    input.MedicalHistory.DateOfReceipt,
                    input.MedicalHistory.Note
                );

            Console.WriteLine($"📋 [PatientAppService] Создана MedicalHistory: Id={medicalHistory.Id}, Department={medicalHistory.NumberDepartment}, Type={medicalHistory.HospitalizationType.DisplayName}");

            CitizenshipInfo citizenshipInfo =
                new(
                    input.CitizenshipInfo.Citizenship,
                    input.CitizenshipInfo.Country,
                    input.CitizenshipInfo.Registration,
                    input.CitizenshipInfo.EarlyRegistration,
                    input.CitizenshipInfo.PlaceOfBirth,
                    input.CitizenshipInfo.DocumentAttached
                );

            Console.WriteLine($"🏛️ [PatientAppService] Создана CitizenshipInfo: Citizenship={citizenshipInfo.Citizenship}, Country={citizenshipInfo.Country}");

            Capable? capable = null;
            if (!input.IsCapable && input.Capable != null)
            {
                capable = new Capable(input.Capable.CourtDecision, input.Capable.TrialDate, input.Capable.Guardian, input.Capable.GuardianOrderAppointment);
                Console.WriteLine($"🧠 [PatientAppService] Создан объект Capable для недееспособного пациента");
            }
            else
            {
                Console.WriteLine($"🧠 [PatientAppService] Capable остается null (пациент дееспособен или данные не заполнены)");
            }

            Pension? pension = null;
            if (input.ReceivesPension && input.Pension != null)
            {
                pension = new Pension(input.Pension.DisabilityGroup, input.Pension.PensionStartDateTime!.Value, input.Pension.PensionAddress, input.Pension.SfrBranch, input.Pension.SfrDepartment, input.Pension.Rsd);
                Console.WriteLine($"💰 [PatientAppService] Создан объект Pension для пациента, получающего пенсию");
            }
            else
            {
                Console.WriteLine($"💰 [PatientAppService] Pension остается null (пациент не получает пенсию или данные не заполнены)");
            }

            var patient = new Patient(
                Guid.NewGuid(),
                input.FullName,
                input.Birthday,
                medicalHistory,
                citizenshipInfo,
                capable, // Теперь корректно передаем null или объект
                pension, // Теперь корректно передаем null или объект
                input.Note
            );

            Console.WriteLine($"👤 [PatientAppService] Создан объект Patient с ID: {patient.Id}");
            Console.WriteLine($"📋 [PatientAppService] MedicalHistories count: {patient.MedicalHistories.Count}");
            Console.WriteLine($"🏛️ [PatientAppService] CitizenshipInfo: {patient.CitizenshipInfo.Citizenship}");
            Console.WriteLine($"🧠 [PatientAppService] Capable: {patient.Capable != null}");
            Console.WriteLine($"💰 [PatientAppService] Pension: {patient.Pension != null}");

            foreach (var documentDto in input.Documents)
            {
                var document = DocumentFactory.Create(documentDto.Key, documentDto.Value.Number);
                patient.AddDocument(document);
                Console.WriteLine($"📄 [PatientAppService] Добавлен документ: {documentDto.Key} = {documentDto.Value.Number}");
            }

            await _unitOfWork.Patients.InsertAsync(patient, cancellationToken);
            Console.WriteLine($"💾 [PatientAppService] Пациент сохранен в базу данных");

            // Публикуем доменные события
            await DomainEventPublisher.PublishAndClearEventsAsync(patient, _domainEventDispatcher, cancellationToken);

            return patient.Id;
        }

        public async Task EditPatientAsync(Guid id, EditPatientDTO input, CancellationToken cancellationToken = default)
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            var patient = await GetPatientByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();

            if (patient.FullName != input.FullName)
                patient.ChangeFullName(input.FullName);

            patient.ChangeCitizenshipInfo(input.CitizenshipInfo.Citizenship, input.CitizenshipInfo.Country, input.CitizenshipInfo.Registration, input.CitizenshipInfo.EarlyRegistration, input.CitizenshipInfo.PlaceOfBirth, input.CitizenshipInfo.DocumentAttached);

            //TODO: Доделать добавление/обновление документов

            Capable? capable = input.Capable != null
                ? new(patient.Capable?.CourtDecision ?? string.Empty, patient.Capable?.TrialDate ?? DateTime.MinValue, input.Capable.Guardian, input.Capable.GuardianOrderAppointment)
                : null;

            if (patient.Capable != null && capable != null)
            {
                if (patient.Capable.CourtDecision != capable.CourtDecision || 
                    patient.Capable.TrialDate != capable.TrialDate ||
                    patient.Capable.Guardian != capable.Guardian ||
                    patient.Capable.GuardianOrderAppointment != capable.GuardianOrderAppointment)
                {
                    patient.SetCapable(capable);
                }
            }
            else if (patient.Capable != null || capable != null)
            {
                patient.SetCapable(capable);
            }

            Pension? pension = input.Pension != null
                ? new(input.Pension.DisabilityGroup, input.Pension.PensionStartDateTime!.Value, input.Pension.PensionAddress, input.Pension.SfrBranch, input.Pension.SfrDepartment, input.Pension.Rsd)
                : null;

            if (patient.Pension != null && pension != null)
            {
                if (patient.Pension.DisabilityGroup?.Value != pension.DisabilityGroup?.Value ||
                    patient.Pension.PensionStartDateTime != pension.PensionStartDateTime ||
                    patient.Pension.PensionAddress?.Value != pension.PensionAddress?.Value ||
                    patient.Pension.SfrBranch != pension.SfrBranch ||
                    patient.Pension.SfrDepartment != pension.SfrDepartment ||
                    patient.Pension.Rsd != pension.Rsd)
                {
                    patient.SetPension(pension);
                }
            }
            else if (patient.Pension != null || pension != null)
            {
                patient.SetPension(pension);
            }

            if (patient.Note != input.Note)
                patient.SetNote(input.Note);

            await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);

            // Публикуем доменные события
            await DomainEventPublisher.PublishAndClearEventsAsync(patient, _domainEventDispatcher, cancellationToken);
        }

        public async Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await GetPatientByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
            // Вызываем доменный метод для мягкого удаления
            patient.SoftDelete();
            
            await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);

            // Публикуем доменные события
            await DomainEventPublisher.PublishAndClearEventsAsync(patient, _domainEventDispatcher, cancellationToken);
        }

        /// <summary>
        /// Получить активных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<ActivePatientDTO>> GetActivePatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var patients = await _unitOfWork.Patients.GetActivePatientsForDataTableAsync(parameters, cancellationToken);
            
            var dtos = patients.Data.Select(patient => new ActivePatientDTO
            {
                Id = patient.Id,
                HospitalizationType = patient.ActiveHistory?.HospitalizationType.ToString() ?? string.Empty,
                CourtDecision = patient.Capable?.CourtDecision,
                NumberDocument = patient.ActiveHistory?.NumberDocument ?? string.Empty,
                DateOfReceipt = patient.ActiveHistory?.DateOfReceipt ?? DateTime.MinValue,
                Department = patient.ActiveHistory?.NumberDepartment.ToString() ?? string.Empty,
                FullName = patient.FullName,
                Birthday = patient.Birthday,
                IsChildren = patient.IsChildren,
                Citizenship = patient.CitizenshipInfo.Citizenship.ToString(),
                Country = patient.CitizenshipInfo.Country,
                Registration = patient.CitizenshipInfo.Registration,
                IsHomeless = patient.CitizenshipInfo.NotRegistered,
                EarlyRegistration = patient.CitizenshipInfo.EarlyRegistration?.DisplayName,
                PlaceOfBirth = patient.CitizenshipInfo.PlaceOfBirth,
                IsCapable = patient.IsCapable,
                ReceivesPension = patient.ReceivesPension,
                DisabilityGroup = patient.Pension?.DisabilityGroup != null && patient.Pension.DisabilityGroup.Value > 0 ? patient.Pension.DisabilityGroup.Value.ToString() : null,
                Note = patient.Note
            }).ToList();

            return new DataTableResult<ActivePatientDTO>
            {
                Data = dtos,
                TotalRecords = patients.TotalRecords,
                FilteredRecords = patients.FilteredRecords
            };
        }

        /// <summary>
        /// Получить архивных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetArchivedPatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Patients.GetArchivedPatientsForDataTableAsync(parameters, cancellationToken);
        }

        private static string GetDocumentNumber(DocumentType? document)
        {
            if (document == null) return string.Empty;
            
            return document switch
            {
                PassportDocument passport => passport.Number ?? string.Empty,
                MedicalPolicyDocument policy => policy.Number ?? string.Empty,
                SnilsDocument snils => snils.Number ?? string.Empty,
                _ => string.Empty
            };
        }
    }
}
