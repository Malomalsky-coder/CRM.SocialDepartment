using CRM.SocialDepartment.Domain.Exceptions;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public sealed class MedicalPolicyDocument : Document
    {
        public override string DisplayName => "Полис ОМС";

        public MedicalPolicyDocument(string number) : base(number)
        {
            if (!IsValid())
                throw new InvalidDocumentNumberException(GetType().Name, number);

            Number = number;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Number) && Number.Length == 16;
        }
    }
}
