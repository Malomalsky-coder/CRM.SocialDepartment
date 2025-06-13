using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Application.DTOs
{
    public record DocumentDTO
    {
        public required DocumentType Type { get; init; }
        public required string Number { get; init; }
    }
}
