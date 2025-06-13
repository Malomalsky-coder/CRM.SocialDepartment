using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    /// <summary>
    /// Модель модального окна для редактирования пациента
    /// </summary>
    public class EditPatientViewModel : PatientViewModelBase
    {
        public required Guid PatientId { get; init; }

        /// <summary>
        /// Дата рождения пациента
        /// </summary>
        [DisplayName("Дата рождения")]
        public DateTime Birthday { get; }

        [DisplayName("Несовершеннолетний")]
        public bool IsChildren { get; private set; } //todo: уточнить про модификатор private set, чтобы было автообновление, без ручного обновления

        // Вспомогательные флаги для восстановления состояния формы редактирования ///////////////////////////////////////////////////

        [BindNever]
        public required string NoRegistrationIsEnable { get; set; }

        [BindNever]
        public required string CountryIsEnable { get; set; }

        [BindNever]
        public required string RegistrationIsEnable { get; set; }

        [BindNever]
        public required string EarlyRegistrationIsEnable { get; set; }

        [BindNever]
        public required string LbgIsEnable { get; set; }

        [BindNever]
        public required string DocumentIsEnable { get; set; }

        [BindNever]
        public required string CapableIsEnable { get; set; }

        [BindNever]
        public required string PensionFieldsetIsEnable { get; set; }

        [BindNever]
        public required string PensionStartDateTimeIsEnable { get; set; }
    }
}