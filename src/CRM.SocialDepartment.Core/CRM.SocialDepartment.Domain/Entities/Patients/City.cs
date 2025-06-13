using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class City : ValueObject
    {
        public static readonly City None            = new(0, "Выберите значение");
        public static readonly City Moscow          = new(1, "Москва");
        public static readonly City FromAnotherTown = new(2, "Иногородний");

        public byte Value { get; }
        public string DisplayName { get; }

        private City(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static City FromValue(byte value)
        {
            return value switch
            {
                0 => None,
                1 => Moscow,
                2 => FromAnotherTown,
                _ => throw new ArgumentException($"Недопустимое значение для City: {value}")
            };
        }

        public static City FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Выберите значение" => None,
                "Москва"            => Moscow,
                "Иногородний"       => FromAnotherTown,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для City: {displayName}")
            };
        }

        public static implicit operator byte(City city) => city.Value;
        public static implicit operator City(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}