using CRM.SocialDepartment.Domain.Entities.Patients;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.Models.Patient
{
    /// <summary>
    /// Пенсия
    /// </summary>
    public class PensionModel
    {
        /// <summary>
        /// Группа инвалидности
        /// </summary>
        [DisplayName("Группа инвалидности")]
        public DisabilityGroupType DisabilityGroup { get; init; }

        /// <summary>
        /// С какого числа установлен статус пенсионера
        /// </summary>
        [DisplayName("Дата установления статуса пенсионера")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PensionStartDateTime { get; init; }

        /// <summary>
        /// Способ получения пенсии
        /// </summary>
        [DisplayName("Способ получения пенсии")]
        public PensionAddressType PensionAddress { get; init; }

        /// <summary>
        /// Филиал СФР
        /// </summary>
        [DisplayName("Филиал СФР")]
        public int SfrBranch { get; init; }

        /// <summary>
        /// Отделение СФР
        /// </summary>
        [DisplayName("Отделение СФР")]
        public string SfrDepartment { get; init; }

        /// <summary>
        /// РСД
        /// </summary>
        [DisplayName("РСД")]
        public string? Rsd { get; init; }
    }
}
