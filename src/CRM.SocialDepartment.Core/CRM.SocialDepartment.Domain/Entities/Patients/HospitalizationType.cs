using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Domain.Entities.Patients
{
    /// <summary>
    /// Тип госпитализации
    /// </summary>
    public enum HospitalizationType : byte
    {
        /// <summary>
        /// Принудительно
        /// </summary>
        [Display(Name = "Принудительно")]
        Force,

        /// <summary>
        /// Добровольный
        /// </summary>
        [Display(Name = "Добровольный")]
        Voluntary,

        /// <summary>
        /// Статья 435 УК РФ
        /// </summary>
        [Display(Name = "Статья 435 УК РФ")]
        YKRFArticle435
    }
}
