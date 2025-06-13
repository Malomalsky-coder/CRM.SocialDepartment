using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.ViewModels.Patient;

namespace CRM.SocialDepartment.Site.Helpers
{
    public static class DocumentViewModelHelper
    {
        public static DocumentViewModel CreateViewModel(DocumentType type, Document? document = null)
        {
            return new DocumentViewModel
            {
                Type = type,
                DisplayName = type switch
                {
                    DocumentType.Passport       => "Паспорт",
                    DocumentType.MedicalPolicy  => "Полис ОМС",
                    DocumentType.Snils          => "СНИЛС",
                    _                           => type.ToString()
                },
                Number = document?.Number ?? string.Empty,
                InputType = "text",
                Placeholder = type switch
                {
                    DocumentType.Passport       => "Серия и номер (например: 1234 567890)",
                    DocumentType.MedicalPolicy  => "Номер полиса (16 цифр)",
                    DocumentType.Snils          => "Номер СНИЛС (например: 123-456-789 01)",
                    _ => string.Empty
                },
                Pattern = type switch
                {
                    DocumentType.Passport       => @"^\d{4}\s\d{6}$",
                    DocumentType.MedicalPolicy  => @"^\d{16}$",
                    DocumentType.Snils          => @"^\d{3}-\d{3}-\d{3}\s\d{2}$",
                    _                           => string.Empty
                },
                ErrorMessage = type switch
                {
                    DocumentType.Passport       => "Введите серию и номер паспорта в формате: 1234 567890",
                    DocumentType.MedicalPolicy  => "Введите 16 цифр номера полиса",
                    DocumentType.Snils          => "Введите номер СНИЛС в формате: 123-456-789 01",
                    _                           => string.Empty
                }
            };
        }
    }
}
