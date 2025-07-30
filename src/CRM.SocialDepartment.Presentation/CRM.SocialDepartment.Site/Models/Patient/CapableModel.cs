using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.Models.Patient
{
    /// <summary>
    /// Дееспособнный
    /// </summary>
    public class CapableModel
    {
        /// <summary>
        /// Решение суда
        /// </summary>
        [DisplayName("Решение суда")]
        public string CourtDecision { get; init; } = string.Empty;

        /// <summary>
        /// Дата проведения суда
        /// </summary>
        [DisplayName("Дата проведения суда")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TrialDate { get; init; }

        /// <summary>
        /// Опекун
        /// </summary>
        [DisplayName("Опекун")]
        public string Guardian { get; init; } = string.Empty;

        /// <summary>
        /// Распоряжение о назначение опекуна
        /// </summary>
        [DisplayName("Распоряжение о назначении опекуна")]
        public string GuardianOrderAppointment { get; init; } = string.Empty;
    }
}
