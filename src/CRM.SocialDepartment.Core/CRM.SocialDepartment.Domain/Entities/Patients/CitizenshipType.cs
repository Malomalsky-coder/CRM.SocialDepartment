using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class CitizenshipType : ValueObject
    {
        public static readonly CitizenshipType RussianFederation    = new(0, "Российская Федерация");
        public static readonly CitizenshipType Foreigner            = new(1, "Иностранец");
        public static readonly CitizenshipType StatelessPerson      = new(2, "ЛБГ");

        public byte Value { get; }
        public string DisplayName { get; }

        // Публичный конструктор по умолчанию для MongoDB десериализации
        public CitizenshipType()
        {
            Value = 0;
            DisplayName = "";
        }

        private CitizenshipType(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static CitizenshipType FromValue(byte value)
        {
            return value switch
            {
                0 => RussianFederation,
                1 => Foreigner,
                2 => StatelessPerson,
                _ => throw new ArgumentException($"Недопустимое значение для CitizenshipType: {value}")
            };
        }

        public static CitizenshipType FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Российская Федерация"  => RussianFederation,
                "РФ"                    => RussianFederation,
                "Иностранец"            => Foreigner,
                "ЛБГ"                   => StatelessPerson,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для CitizenshipType: {displayName}")
            };
        }

        public static implicit operator byte(CitizenshipType citizenshipType) => citizenshipType.Value;
        public static implicit operator CitizenshipType(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}
