using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    public enum City : byte
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        [Display(Name = "Выберите значение")]
        None,

        /// <summary>
        /// Москва
        /// </summary>
        [Display(Name = "Москва")]
        Moscow,

        /// <summary>
        /// Иногородний
        /// </summary>
        [Display(Name = "Иногородний")]
        FromAnotherTown
    }
}
