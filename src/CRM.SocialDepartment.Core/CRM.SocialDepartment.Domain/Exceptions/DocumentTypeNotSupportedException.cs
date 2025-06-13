namespace CRM.SocialDepartment.Domain.Exceptions
{
    public class DocumentTypeNotSupportedException : DomainException
    {
        public DocumentTypeNotSupportedException(string documentType)
            : base($"Неподдерживаемый тип документа: {documentType}")
        {
        }
    }
}
