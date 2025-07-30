using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Exceptions;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Factories
{
    public static class DocumentFactory
    {
        public static DocumentType Create(DocumentType documentType, string? number)
        {
            object result = documentType.Value switch
            {
                0 => new PassportDocument(number),
                1 => new MedicalPolicyDocument(number),
                2 => new SnilsDocument(number),
                _ => throw new DocumentTypeNotSupportedException($"Неподдерживаемый тип документа: {documentType}")
            };
            
            return (DocumentType)result;
        }
    }
}
