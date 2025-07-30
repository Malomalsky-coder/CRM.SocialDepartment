using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace CRM.SocialDepartment.Site.Models.Patient
{
    /// <summary>
    /// Информация о гражданстве
    /// </summary>
    public class CitizenshipInfoModel
    {
        [BindNever]
        [DisplayName("Список гражданств")]
        public string[] Citizenships { get; init; }

        /// <summary>
        /// Гражданство
        /// </summary>
        [DisplayName("Гражданство")]
        public CitizenshipType? Citizenship { get; init; }

        /// <summary>
        /// Страна
        /// </summary>
        [DisplayName("Страна")]
        public string? Country { get; init; }

        /// <summary>
        /// Место регистрации
        /// </summary>
        [DisplayName("Место регистрации")]
        public string? Registration { get; init; }

        /// <summary>
        /// Нет регистрации, бомж
        /// </summary>
        [DisplayName("Нет регистрации")]
        public bool NotRegistered { get; init; }

        /// <summary>
        /// Ранняя регистрация
        /// </summary>
        [DisplayName("Ранняя регистрация")]
        public CityType? EarlyRegistration { get; init; }

        /// <summary>
        /// Место рождения
        /// </summary>
        [DisplayName("Место рождения")]
        public string? PlaceOfBirth { get; init; }

        /// <summary>
        /// Имеющиеся документы
        /// </summary>
        [DisplayName("Имеющиеся документы")]
        public string DocumentAttached { get; init; }
    }
}
