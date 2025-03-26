using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class CreateOrEditPatientDTO
    {
        public string FullName { get; set; }
        public DateTime Birthday { get; set; } //TODO: Нельзя изменить, значит вынести в EditPatientDTO без данного свойства
        public CitizenshipInfoDTO CitizenshipInfo { get; set; }
        public Dictionary<Document, object> Documents { get; set; } = [];
        public CapableDTO? Capable { get; set; }
        public PensionDTO? Pension { get; set; }
        public string? Note { get; set; }
    }

    public class CitizenshipInfoDTO
    {
        public CitizenshipType Citizenship { get; set; }
        public string? Country { get; set; }
        public string? Registration { get; set; }
        public City? EarlyRegistration { get; set; }
        public string? PlaceOfBirth { get; set; }
    }

    public class CapableDTO
    {
        public string CourtDecision { get; set; }   //TODO: Нельзя изменить, значит вынести в EditPatientDTO без данного свойства
        public DateTime TrialDate { get; set; }     //TODO: Нельзя изменить, значит вынести в EditPatientDTO без данного свойства
        public string Guardian { get; set; }
        public string GuardianOrderAppointment { get; set; }
    }

    public class PensionDTO
    {
        public DisabilityGroup DisabilityGroup { get; set; }
        public DateTime PensionStartDateTime { get; set; }
        public PensionAddress PensionAddress { get; set; }
        public int SfrBranch { get; set; }
        public string SfrDepartment { get; set; }
        public string? Rsd { get; set; }
    }
}
