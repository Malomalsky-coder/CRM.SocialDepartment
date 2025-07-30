using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class DocumentDTO
    {
        public DocumentType Type { get; init; } = DocumentType.Passport;
        public string Number { get; init; } = string.Empty;
    }
}
