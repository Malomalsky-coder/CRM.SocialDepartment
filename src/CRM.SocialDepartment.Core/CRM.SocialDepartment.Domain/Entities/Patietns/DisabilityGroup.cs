using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patietns
{
    public enum DisabilityGroup : byte
    {
        /// <summary>
        /// Значение не выбрано
        /// </summary>
        [Display(Name = "Выберите значение")]
        None,

        /// <summary>
        /// 1 группа
        /// </summary>
        [Display(Name = "1 группа")]
        OneGroup,

        /// <summary>
        /// 1 группа б/с
        /// </summary>
        [Display(Name = "1 группа б/с")]
        OneGroupWithoutPeriod,

        /// <summary>
        /// 2 группа
        /// </summary>
        [Display(Name = "2 группа")]
        TwoGroup,

        /// <summary>
        /// 2 группа б/с
        /// </summary>
        [Display(Name = "2 группа б/с")]
        TwoGroupWithoutPeriod,

        /// <summary>
        /// 3 группа
        /// </summary>
        [Display(Name = "3 группа")]
        ThreeGroup,

        /// <summary>
        /// 3 группа б/с
        /// </summary>
        [Display(Name = "3 группа б/с")]
        ThreeGroupWithoutPeriod,

        /// <summary>
        /// Ребенок инвалид
        /// </summary>
        [Display(Name = "Ребенок инвалид")]
        ChildrenDisable
    }
}
