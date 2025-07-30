using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class CityType : ValueObject
    {
        public static readonly CityType None            = new(0, "Выберите значение");
        public static readonly CityType Moscow          = new(1, "Москва");
        public static readonly CityType FromAnotherTown = new(2, "Иногородний");

        public byte Value { get; }
        public string DisplayName { get; }

        private CityType(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static CityType FromValue(byte value)
        {
            return value switch
            {
                0 => None,
                1 => Moscow,
                2 => FromAnotherTown,
                _ => throw new ArgumentException($"Недопустимое значение для City: {value}")
            };
        }

        public static CityType FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Выберите значение" => None,
                "Москва"            => Moscow,
                "Иногородний"       => FromAnotherTown,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для City: {displayName}")
            };
        }

        public static implicit operator byte(CityType city) => city.Value;
        public static implicit operator CityType(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}