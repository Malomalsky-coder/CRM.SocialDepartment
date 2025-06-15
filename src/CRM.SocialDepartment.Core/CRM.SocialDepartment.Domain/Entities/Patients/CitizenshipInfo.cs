using DDD.Values;
using System.ComponentModel;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Информация о гражданстве
    /// </summary>
    public class CitizenshipInfo : ValueObject
    {
        /// <summary>
        /// Гражданство
        /// </summary>
        public CitizenshipType Citizenship { get; private set; }

        /// <summary>
        /// Страна
        /// </summary>
        public string? Country { get; private set; }

        /// <summary>
        /// Место регистрации
        /// </summary>
        public string? Registration { get; private set; }

        /// <summary>
        /// Нет регистрации, бомж
        /// </summary>
        public bool NotRegistered => Registration is null;

        /// <summary>
        /// Ранняя регистрация
        /// </summary>
        public City? EarlyRegistration { get; private set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        public string? PlaceOfBirth { get; private set; }

        /// <summary>
        /// Имеющиеся документы
        /// </summary>
        public string? DocumentAttached { get; private set; }

        #pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private CitizenshipInfo() { }
        #pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public CitizenshipInfo(
            CitizenshipType citizenship,
            string? country,
            string? registration,
            City? earlyRegistration,
            string? placeOfBirth,
            string? documentAttached)
        {
            Citizenship = citizenship;
            Country = country;
            Registration = registration;
            EarlyRegistration = earlyRegistration;
            PlaceOfBirth = placeOfBirth;
            DocumentAttached = documentAttached;
        }

        /// <summary>
        /// Изменить гражданство
        /// </summary>
        /// <param name="citizenship">Новое гражданство</param>
        public CitizenshipInfo SetCitizenship(CitizenshipType citizenship)
        {
            Citizenship = citizenship;
            return this;
        }

        /// <summary>
        /// Изменить страну
        /// </summary>
        /// <param name="country">Новая страна</param>
        public CitizenshipInfo SetCountry(string? country)
        {
            Country = country;
            return this;
        }

        /// <summary>
        /// Изменить место регистрации
        /// </summary>
        /// <param name="registration">Новое место регистрации</param>
        public CitizenshipInfo SetRegistration(string? registration)
        {
            Registration = registration;
            return this;
        }

        /// <summary>
        /// Изменить раннюю регистрацию
        /// </summary>
        /// <param name="earlyRegistration">Новая ранняя регистрация</param>
        public CitizenshipInfo SetEarlyRegistration(City? earlyRegistration)
        {
            EarlyRegistration = earlyRegistration;
            return this;
        }

        /// <summary>
        /// Изменить место рождения
        /// </summary>
        /// <param name="placeOfBirth">Новое место рождения</param>
        public CitizenshipInfo SetPlaceOfBirth(string? placeOfBirth)
        {
            PlaceOfBirth = placeOfBirth;
            return this;
        }

        /// <summary>
        /// Изменить имеющиеся документы
        /// </summary>
        /// <param name="documentAttached">Новые имеющиеся документы</param>
        public CitizenshipInfo SetDocumentAttached(string? documentAttached)
        {
            DocumentAttached = documentAttached;
            return this;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Citizenship;
            yield return Country ?? string.Empty;
            yield return Registration ?? string.Empty;
            yield return NotRegistered;
            yield return EarlyRegistration ?? City.None;
            yield return PlaceOfBirth ?? string.Empty;
            yield return DocumentAttached ?? string.Empty;
        }
    }
}
