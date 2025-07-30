using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.ViewModels.Patient;

namespace CRM.SocialDepartment.Site.Helpers
{
    public static class DocumentHelper
    {
        public static DocumentViewModel CreateViewModel(DocumentType documentType)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport => new DocumentViewModel
                {
                    Type = DocumentType.Passport,
                    Number = "",
                    DisplayName = "Паспорт",
                    Placeholder = "Серия и номер (например: 1234 567890)",
                    Pattern = @"^\d{4}\s\d{6}$",
                    ErrorMessage = "Введите серию и номер паспорта в формате: 1234 567890"
                },
                var dt when dt == DocumentType.MedicalPolicy => new DocumentViewModel
                {
                    Type = DocumentType.MedicalPolicy,
                    Number = "",
                    DisplayName = "Полис ОМС",
                    Placeholder = "Номер полиса (16 цифр)",
                    Pattern = @"^\d{16}$",
                    ErrorMessage = "Введите 16 цифр номера полиса"
                },
                var dt when dt == DocumentType.Snils => new DocumentViewModel
                {
                    Type = DocumentType.Snils,
                    Number = "",
                    DisplayName = "СНИЛС",
                    Placeholder = "Номер СНИЛС (например: 123-456-789 01)",
                    Pattern = @"^\d{3}-\d{3}-\d{3}\s\d{2}$",
                    ErrorMessage = "Введите номер СНИЛС в формате: 123-456-789 01"
                },
                _ => throw new ArgumentException($"Неизвестный тип документа: {documentType}")
            };
        }
    }
}
