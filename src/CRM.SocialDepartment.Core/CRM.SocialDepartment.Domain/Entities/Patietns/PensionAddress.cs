using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patietns
{
    /// <summary>
    /// Способ получения пенсии
    /// </summary>
    public enum PensionAddress : byte
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        [Display(Name = "Выберите значение")]
        None,

        /// <summary>
        /// ПКБ
        /// </summary>
        [Display(Name = "ПКБ №5")]
        PHC5,

        /// <summary>
        /// ОСБ
        /// </summary>
        [Display(Name = "ОСБ")]
        OSB,

        /// <summary>
        /// Место жительства
        /// </summary>
        [Display(Name = "Место жительства")]
        Registration
    }
}
