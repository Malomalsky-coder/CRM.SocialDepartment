﻿using DDD.Values;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Тип госпитализации
    /// </summary>
    public class HospitalizationType : ValueObject
    {
        /// <summary>
        /// Принудительно
        /// </summary>
        public static readonly HospitalizationType Force = new(0, "Принудительно");

        /// <summary>
        /// Добровольный
        /// </summary>
        public static readonly HospitalizationType Voluntary = new(1, "Добровольный");

        /// <summary>
        /// Статья 435 УК РФ
        /// </summary>
        public static readonly HospitalizationType CriminalCodeRFArticle435 = new (2, "Статья 435 УК РФ");

        public byte Value { get; }
        public string DisplayName { get; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private HospitalizationType()
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        {
            
        }

        public HospitalizationType(byte value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static HospitalizationType FromValue(byte value)
        {
            return value switch
            {
                0 => Force,
                1 => Voluntary,
                2 => CriminalCodeRFArticle435,
                _ => throw new ArgumentException($"Недопустимое значение для HospitalizationType: {value}")
            };
        }

        public static HospitalizationType FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "Принудительно" => Force,
                "Добровольный" => Voluntary,
                "Статья 435 УК РФ" => CriminalCodeRFArticle435,
                _ => throw new ArgumentException($"Недопустимое отображаемое имя для HospitalizationType: {displayName}")
            };
        }

        public static implicit operator byte(HospitalizationType hospitalizationType) => hospitalizationType.Value;
        public static implicit operator HospitalizationType(byte value) => FromValue(value);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => DisplayName;
    }
}
