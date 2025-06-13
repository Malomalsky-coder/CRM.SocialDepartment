using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.ViewModels.Patient;

namespace CRM.SocialDepartment.Site.Helpers
{
    public static class DocumentHelper
    {
        public static DocumentViewModel CreateViewModel(DocumentType type)
        {
            return type switch
            {
                DocumentType.Passport => new DocumentViewModel
                {
                    Type = DocumentType.Passport,
                    DisplayName = "Паспорт",
                    InputType = "text",
                    Placeholder = "Серия и номер (например: 1234 567890)",
                    Pattern = @"^\d{4}\s\d{6}$",
                    ErrorMessage = "Введите серию и номер паспорта в формате: 1234 567890"
                },
                DocumentType.MedicalPolicy => new DocumentViewModel
                {
                    Type = DocumentType.MedicalPolicy,
                    DisplayName = "Полис ОМС",
                    InputType = "text",
                    Placeholder = "Номер полиса (16 цифр)",
                    Pattern = @"^\d{16}$",
                    ErrorMessage = "Введите 16 цифр номера полиса"
                },
                DocumentType.Snils => new DocumentViewModel
                {
                    Type = DocumentType.Snils,
                    DisplayName = "СНИЛС",
                    InputType = "text",
                    Placeholder = "Номер СНИЛС (например: 123-456-789 01)",
                    Pattern = @"^\d{3}-\d{3}-\d{3}\s\d{2}$",
                    ErrorMessage = "Введите номер СНИЛС в формате: 123-456-789 01"
                },
                _ => throw new ArgumentException($"Неподдерживаемый тип документа: {type}")
            };
        }
    }
}
