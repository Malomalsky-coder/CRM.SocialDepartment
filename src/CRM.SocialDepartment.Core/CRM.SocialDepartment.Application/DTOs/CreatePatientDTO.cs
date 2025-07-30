using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreatePatientDTO
    {
        public string FullName { get; init; } = string.Empty;
        public DateTime Birthday { get; init; }
        public MedicalHistoryDTO MedicalHistory { get; init; } = new()
        {
            NumberDepartment = 0,
            HospitalizationType = new HospitalizationTypeDTO { Value = 0, DisplayName = "Принудительно" },
            DateOfReceipt = DateTime.Now
        };
        public CitizenshipInfoDTO CitizenshipInfo { get; init; } = new()
        {
            Citizenship = Domain.Entities.Patients.CitizenshipType.RussianFederation
        };
        public Dictionary<DocumentType, DocumentDTO>? Documents { get; init; }
        public bool IsCapable { get; init; } = true;
        public CreateCapableDTO? Capable { get; init; }
        public bool ReceivesPension { get; init; } = false;
        public PensionDTO? Pension { get; init; }
        public string? Note { get; init; }
    }
}
