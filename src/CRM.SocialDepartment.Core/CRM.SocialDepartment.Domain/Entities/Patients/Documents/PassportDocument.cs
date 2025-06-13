using CRM.SocialDepartment.Domain.Exceptions;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    public sealed class PassportDocument : Document
    {
        public override string DisplayName => "Паспорт";

        //Пример будущего расширения
        //public string Series { get; private set; } // Серия (например, "4500")

        public PassportDocument(string number) : base(number)
        {
            if (!IsValid())
                throw new InvalidDocumentNumberException(GetType().Name, number);

            Number = number;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Number) && Number.Length == 11 && Number.All(char.IsDigit);
        }

        //Если расширять тип, то необходимо переопределить метод
        //protected override IEnumerable<object> GetAtomicValues()
        //{
        //    yield return Series;
        //    yield return Number;
        //}
    }
}
