using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patietns
{
    public enum Document : byte
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        [Display(Name = "Выберите значение")]
        None,

        /// <summary>
        /// Паспорт
        /// </summary>
        [Display(Name = "Паспорт")]
        Passport,

        /// <summary>
        /// Медицинский полис
        /// </summary>
        [Display(Name = "Медицинский полис")]
        MedicalPolicy,

        /// <summary>
        /// СНИЛС
        /// </summary>
        [Display(Name = "СНИЛС")]
        Snils
    }
}
