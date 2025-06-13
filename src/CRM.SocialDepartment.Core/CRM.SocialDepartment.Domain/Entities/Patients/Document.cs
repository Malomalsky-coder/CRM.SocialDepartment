using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public abstract class Document : ValueObject
    {
        public string Number { get; protected set; }

        protected Document(string number)
        {
            Number = number;
        }

        public abstract string DisplayName { get; }
        public abstract bool IsValid();

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number;
        }
    }
}
