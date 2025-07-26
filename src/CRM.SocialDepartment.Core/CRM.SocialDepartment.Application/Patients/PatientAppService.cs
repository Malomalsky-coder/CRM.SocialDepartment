using AutoMapper;
using CRM.SocialDepartment.Application.Common;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
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

        public async Task<IEnumerable<Patient>> GetAllPatients(Expression<Func<Patient, bool>>? predicate, CancellationToken cancellationToken = default)
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            return await _unitOfWork.Patients.GetAllAsync(predicate, cancellationToken);
        }

        public async Task<Guid> AddPatientAsync(CreatePatientDTO input, CancellationToken cancellationToken = default)
        {
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

            CitizenshipInfo citizenshipInfo =
                new(
                    input.CitizenshipInfo.Citizenship,
                    input.CitizenshipInfo.Country,
                    input.CitizenshipInfo.Registration,
                    input.CitizenshipInfo.EarlyRegistration,
                    input.CitizenshipInfo.PlaceOfBirth,
                    input.CitizenshipInfo.DocumentAttached
                );

            Capable? capable = input.Capable != null
                ? new Capable(input.Capable.CourtDecision, input.Capable.TrialDate, input.Capable.Guardian, input.Capable.GuardianOrderAppointment)
                : null;

            Pension? pension = input.Pension != null
                ? new Pension(input.Pension.DisabilityGroup, input.Pension.PensionStartDateTime, input.Pension.PensionAddress, input.Pension.SfrBranch, input.Pension.SfrDepartment, input.Pension.Rsd)
                : null;

            var patient = new Patient(
                Guid.NewGuid(),
                input.FullName,
                input.Birthday,
                medicalHistory,
                citizenshipInfo,
                capable,
                pension,
                input.Note
            );

            foreach (var documentDto in input.Documents)
            {
                var document = DocumentFactory.Create(documentDto.Key, documentDto.Value.Number);
                patient.AddDocument(document);
            }

            await _unitOfWork.Patients.InsertAsync(patient, cancellationToken);

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
                ? new(patient.Capable!.CourtDecision, patient.Capable.TrialDate, input.Capable.Guardian, input.Capable.GuardianOrderAppointment)
                : null;

            if (patient.Capable?.Equals(capable) != true)
                patient.SetCapable(capable);

            Pension? pension = input.Pension != null
                ? new(input.Pension.DisabilityGroup, input.Pension.PensionStartDateTime, input.Pension.PensionAddress, input.Pension.SfrBranch, input.Pension.SfrDepartment, input.Pension.Rsd)
                : null;

            if (patient.Pension?.Equals(pension) != true)
                patient.SetPension(pension);

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
        public async Task<DataTableResult<Patient>> GetActivePatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Patients.GetActivePatientsForDataTableAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Получить архивных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetArchivedPatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Patients.GetArchivedPatientsForDataTableAsync(parameters, cancellationToken);
        }
    }
}
