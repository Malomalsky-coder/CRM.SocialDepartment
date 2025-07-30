using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients.Documents
{
    /// <summary>
    /// Базовый класс для типов документов
    /// </summary>
    public class DocumentType : ValueObject
    {
        public byte Value { get; }
        public virtual string? DisplayName { get; }

        // Статические экземпляры для типов документов
        public static readonly DocumentType Passport = new(0, "Паспорт");
        public static readonly DocumentType MedicalPolicy = new(1, "Медицинский полис");
        public static readonly DocumentType Snils = new(2, "СНИЛС");

        protected DocumentType()
        {
            DisplayName = null;
        }

        protected DocumentType(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static DocumentType FromValue(byte value)
        {
            return value switch
            {
                0 => Passport,
                1 => MedicalPolicy,
                2 => Snils,
                _ => throw new ArgumentException($"Недопустимое значение для DocumentType: {value}")
            };
        }

        public static DocumentType FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Паспорт" => Passport,
                "Медицинский полис" => MedicalPolicy,
                "СНИЛС" => Snils,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для DocumentType: {displayName}")
            };
        }

        public static implicit operator byte(DocumentType documentType) => documentType.Value;
        public static implicit operator DocumentType(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName ?? "Неизвестный документ";
    }
} 