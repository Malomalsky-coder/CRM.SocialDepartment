using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class PensionDTO
    {
        public required DisabilityGroupType DisabilityGroup { get; init; }
        public DateTime? PensionStartDateTime { get; init; }
        public required PensionAddressType PensionAddress { get; init; }
        public required int SfrBranch { get; init; }
        public required string SfrDepartment { get; init; }
        public string? Rsd { get; init; }
    }
}
