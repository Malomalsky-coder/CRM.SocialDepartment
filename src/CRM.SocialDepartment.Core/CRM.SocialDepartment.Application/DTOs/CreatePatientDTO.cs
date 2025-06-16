#nullable disable
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreatePatientDTO
    {
        public string FullName { get; init; }
        public DateTime Birthday { get; init; }
        public MedicalHistoryDTO MedicalHistory { get; init; }
        public CitizenshipInfoDTO CitizenshipInfo { get; init; }
        public Dictionary<DocumentType, DocumentDTO>? Documents { get; init; }
        public CreateCapableDTO? Capable { get; init; }
        public PensionDTO? Pension { get; init; }
        public string? Note { get; init; }
    }
}
