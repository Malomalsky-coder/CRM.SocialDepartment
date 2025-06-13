namespace CRM.SocialDepartment.Domain.Exceptions
{
    public class InvalidDocumentNumberException : DomainException
    {
        public InvalidDocumentNumberException(string documentType, string number)
            : base($"Некорректный номер документа {documentType}: {number}")
        {
        }
    }
}
