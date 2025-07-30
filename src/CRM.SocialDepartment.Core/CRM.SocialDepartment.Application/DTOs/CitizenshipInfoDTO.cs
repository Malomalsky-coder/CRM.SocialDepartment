using CRM.SocialDepartment.Domain.Entities.Patients;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class CitizenshipInfoDTO
    {
        public required CitizenshipType Citizenship { get; init; }
        public string? Country { get; init; }
        public string? Registration { get; init; }
        public CityType? EarlyRegistration { get; init; }
        public string? PlaceOfBirth { get; init; }
        public string? DocumentAttached { get; init; }
    }
}
