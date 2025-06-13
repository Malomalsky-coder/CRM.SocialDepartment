using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Exceptions;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Factories
{
    public static class DocumentFactory
    {
        public static Document Create(DocumentType type, string number)
        {
            return type switch
            {
                DocumentType.Passport       => new PassportDocument(number),
                DocumentType.MedicalPolicy  => new MedicalPolicyDocument(number),
                DocumentType.Snils          => new SnilsDocument(number),
                _                           => throw new DocumentTypeNotSupportedException($"Неподдерживаемый тип документа: {type}")
            };
        }
    }
}
