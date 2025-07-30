using CRM.SocialDepartment.Domain.Entities.Patients.Documents;

namespace CRM.SocialDepartment.Site.Helpers
{
    public static class DocumentViewModelHelper
    {
        public static string GetDisplayName(DocumentType documentType)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport       => "Паспорт",
                var dt when dt == DocumentType.MedicalPolicy  => "Полис ОМС",
                var dt when dt == DocumentType.Snils          => "СНИЛС",
                _ => throw new ArgumentException($"Неизвестный тип документа: {documentType}")
            };
        }

        public static string GetPlaceholder(DocumentType documentType)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport       => "Серия и номер (например: 1234 567890)",
                var dt when dt == DocumentType.MedicalPolicy  => "Номер полиса (16 цифр)",
                var dt when dt == DocumentType.Snils          => "Номер СНИЛС (например: 123-456-789 01)",
                _ => throw new ArgumentException($"Неизвестный тип документа: {documentType}")
            };
        }

        public static string GetPattern(DocumentType documentType)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport       => @"^\d{4}\s\d{6}$",
                var dt when dt == DocumentType.MedicalPolicy  => @"^\d{16}$",
                var dt when dt == DocumentType.Snils          => @"^\d{3}-\d{3}-\d{3}\s\d{2}$",
                _ => throw new ArgumentException($"Неизвестный тип документа: {documentType}")
            };
        }

        public static string GetErrorMessage(DocumentType documentType)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport       => "Введите серию и номер паспорта в формате: 1234 567890",
                var dt when dt == DocumentType.MedicalPolicy  => "Введите 16 цифр номера полиса",
                var dt when dt == DocumentType.Snils          => "Введите номер СНИЛС в формате: 123-456-789 01",
                _ => throw new ArgumentException($"Неизвестный тип документа: {documentType}")
            };
        }
    }
}
