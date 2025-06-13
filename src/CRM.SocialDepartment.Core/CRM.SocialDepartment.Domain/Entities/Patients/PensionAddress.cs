using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Способ получения пенсии
    /// </summary>
    public class PensionAddress : ValueObject
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        public static readonly PensionAddress None = new(0, "Выберите значение");

        /// <summary>
        /// ПКБ
        /// </summary>
        public static readonly PensionAddress PHC5 = new(1, "ПКБ №5");

        /// <summary>
        /// ОСБ
        /// </summary>
        public static readonly PensionAddress OSB = new(2, "ОСБ");

        /// <summary>
        /// Место жительства
        /// </summary>
        public static readonly PensionAddress Registration = new(3, "Место жительства");

        public byte Value { get; }
        public string DisplayName { get; }

        private PensionAddress(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static PensionAddress FromValue(byte value)
        {
            return value switch
            {
                0 => None,
                1 => PHC5,
                2 => OSB,
                3 => Registration,
                _ => throw new ArgumentException($"Недопустимое значение для PensionAddress: {value}")
            };
        }

        public static PensionAddress FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Выберите значение" => None,
                "ПКБ №5"            => PHC5,
                "ОСБ"               => OSB,
                "Место жительства"  => Registration,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для PensionAddress: {displayName}")
            };
        }

        public static implicit operator byte(PensionAddress pensionAddress) => pensionAddress.Value;
        public static implicit operator PensionAddress(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}
