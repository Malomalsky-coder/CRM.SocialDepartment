using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreatePatientDTO
    {
        public required string FullName { get; init; }
        public required DateTime Birthday { get; init; }
        public required MedicalHistoryDTO MedicalHistory { get; init; }
        public required CitizenshipInfoDTO CitizenshipInfo { get; init; }
        public required Dictionary<DocumentType, DocumentDTO> Documents { get; init; }
        public CreateCapableDTO? Capable { get; init; }
        public PensionDTO? Pension { get; init; }
        public string? Note { get; init; }
    }
}
