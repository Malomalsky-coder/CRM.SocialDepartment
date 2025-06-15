using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class EditPatientDTO
    {
        public required string FullName { get; init; }
        public required MedicalHistoryDTO ActiveMedicalHistory { get; init; }
        public required CitizenshipInfoDTO CitizenshipInfo { get; init; }
        public required Dictionary<DocumentType, DocumentDTO> Documents { get; init; }
        public EditCapableDTO? Capable { get; init; }
        public PensionDTO? Pension { get; init; }
        public string? Note { get; init; }
    }
}
