using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public class DisabilityGroupType : ValueObject
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        public static readonly DisabilityGroupType None = new(0, "Выберите значение");

        /// <summary>
        /// 1 группа
        /// </summary>
        public static readonly DisabilityGroupType OneGroup = new(1, "1 группа");

        /// <summary>
        /// 1 группа б/с
        /// </summary>
        public static readonly DisabilityGroupType OneGroupWithoutPeriod = new(2, "1 группа б/с");

        /// <summary>
        /// 2 группа
        /// </summary>
        public static readonly DisabilityGroupType TwoGroup = new(3, "2 группа");

        /// <summary>
        /// 2 группа б/с
        /// </summary>
        public static readonly DisabilityGroupType TwoGroupWithoutPeriod = new(4, "2 группа б/с");

        /// <summary>
        /// 3 группа
        /// </summary>
        public static readonly DisabilityGroupType ThreeGroup = new(5, "3 группа");

        /// <summary>
        /// 3 группа б/с
        /// </summary>
        public static readonly DisabilityGroupType ThreeGroupWithoutPeriod = new(6, "3 группа б/с");

        /// <summary>
        /// Ребенок инвалид
        /// </summary>
        public static readonly DisabilityGroupType ChildrenDisable = new(7, "Ребенок инвалид");

        public byte Value { get; }
        public string DisplayName { get; }

        private DisabilityGroupType(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static DisabilityGroupType FromValue(byte value)
        {
            return value switch
            {
                0 => None,
                1 => OneGroup,
                2 => OneGroupWithoutPeriod,
                3 => TwoGroup,
                4 => TwoGroupWithoutPeriod,
                5 => ThreeGroup,
                6 => ThreeGroupWithoutPeriod,
                7 => ChildrenDisable,
                _ => throw new ArgumentException($"Недопустимое значение для DisabilityGroup: {value}")
            };
        }

        public static DisabilityGroupType FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Выберите значение" => None,
                "1 группа"          => OneGroup,
                "1 группа б/с"      => OneGroupWithoutPeriod,
                "2 группа"          => TwoGroup,
                "2 группа б/с"      => TwoGroupWithoutPeriod,
                "3 группа"          => ThreeGroup,
                "3 группа б/с"      => ThreeGroupWithoutPeriod,
                "Ребенок инвалид"   => ChildrenDisable,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для DisabilityGroup: {displayName}")
            };
        }

        public static implicit operator byte(DisabilityGroupType disabilityGroup) => disabilityGroup.Value;
        public static implicit operator DisabilityGroupType(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}
