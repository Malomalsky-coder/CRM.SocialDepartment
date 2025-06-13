using CRM.SocialDepartment.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public class SnilsDocument : Document
    {
        public override string DisplayName => "Снилс";


        public SnilsDocument(string number) : base(number)
        {
            if (!IsValid())
                throw new InvalidDocumentNumberException(GetType().Name, number);

            Number = number;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Number) &&
                    Number.Length == 14 &&
                    Regex.IsMatch(Number, @"^\d{3}-\d{3}-\d{3}\s\d{2}$");
        }
    }
}