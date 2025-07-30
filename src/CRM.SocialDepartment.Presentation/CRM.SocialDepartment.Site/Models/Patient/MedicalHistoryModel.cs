using CRM.SocialDepartment.Domain.Entities.Patients;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.Models.Patient
{
    /// <summary>
    /// История болезни
    /// </summary>
    public class MedicalHistoryModel
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        [DisplayName("Номер отделения")]
        public sbyte NumberDepartment { get; init; }

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        [DisplayName("Тип госпитализации")]
        public HospitalizationType HospitalizationType { get; init; }

        /// <summary>
        /// Постановление
        /// </summary>
        [DisplayName("Постановление")]
        public string Resolution { get; init; }

        /// <summary>
        /// Номер истории болезни
        /// </summary>
        [DisplayName("Номер истории болезни")]
        public string NumberDocument { get; init; }

        /// <summary>
        /// Дата поступления
        /// </summary>
        [DisplayName("Дата поступления")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfReceipt { get; init; }

        /// <summary>
        /// Дата выписки
        /// </summary>
        [DisplayName("Дата выписки")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfDischarge { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        [DisplayName("Примечание")]
        public string? Note { get; init; }
    }
}
