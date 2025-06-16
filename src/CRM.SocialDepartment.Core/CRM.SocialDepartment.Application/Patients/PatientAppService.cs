using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Factories;
using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CRM.SocialDepartment.Application.Patients
{
    public class PatientAppService(IMapper mapper, MongoBasicRepository<Patient, Guid> patientRepository)
    {
        private readonly MongoBasicRepository<Patient, Guid> _patientRepository = patientRepository;
        private readonly IMapper _mapper = mapper;

        public IMongoCollection<Patient> GetPatientCollection()
        {
            return _patientRepository.GetCollection();
        }

        public async Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default) //TODO: ??? return PatientDTO
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            return await _patientRepository.GetAsync((item) => item.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Patient>> GetAllPatients(Expression<Func<Patient, bool>>? predicate, CancellationToken cancellationToken = default)
        {
            //TODO: Разрешить доступ по правилу ролей. Примечание: Мед. персонал может получать пациентов только из своего отделения.

            return await _patientRepository.GetAllAsync(predicate, cancellationToken);
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

            await _patientRepository.InsertAsync(patient, cancellationToken);

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

            await _patientRepository.UpdateAsync(patient, cancellationToken);
        }

        public async Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var patient = await GetPatientByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
            await _patientRepository.DeleteAsync(patient, cancellationToken);
        }
    }
}
