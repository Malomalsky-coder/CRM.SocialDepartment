using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class PensionDTO
    {
        public required DisabilityGroup DisabilityGroup { get; init; }
        public DateTime PensionStartDateTime { get; init; }
        public required PensionAddress PensionAddress { get; init; }
        public required int SfrBranch { get; init; }
        public required string SfrDepartment { get; init; }
        public string? Rsd { get; init; }
    }
}
