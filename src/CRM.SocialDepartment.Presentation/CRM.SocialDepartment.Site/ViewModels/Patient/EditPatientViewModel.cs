using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.ViewModels.Validation;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    /// <summary>
    /// Модель модального окна для редактирования пациента
    /// </summary>
    public class EditPatientViewModel : ValidationBase, IValidatableObject
    {
        /// <summary>
        /// Идентификатор пациента
        /// </summary>
        [Required]
        public Guid PatientId { get; init; }

        /// <summary>
        /// Полное имя пациента
        /// </summary>
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "ФИО должно содержать от 3 до 100 символов")]
        [Display(Name = "ФИО")]
        public required string FullName { get; init; }

        /// <summary>
        /// История болезни
        /// </summary>
        [Required(ErrorMessage = "История болезни обязательна для заполнения")]
        [Display(Name = "История болезни")]
        public required MedicalHistory MedicalHistory { get; init; }

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        [Required(ErrorMessage = "Информация о гражданстве обязательна для заполнения")]
        [Display(Name = "Информация о гражданстве")]
        public required CitizenshipInfo CitizenshipInfo { get; init; }

        /// <summary>
        /// Список документов
        /// </summary>
        [Display(Name = "Список документов")]
        public Dictionary<DocumentType, DocumentViewModel>? Documents { get; init; }

        /// <summary>
        /// Пациент является дееспособным?
        /// </summary>
        [Display(Name = "Дееспособность")]
        public required bool IsCapable { get; init; }

        /// <summary>
        /// Дееспособный
        /// </summary>
        [Display(Name = "Информация о дееспособности")]
        public Capable? Capable { get; init; }

        /// <summary>
        /// Получает ли пациент пенсию
        /// </summary>
        [Display(Name = "Получение пенсии")]
        public required bool ReceivesPension { get; init; }

        /// <summary>
        /// Пенсия
        /// </summary>
        [Display(Name = "Информация о пенсии")]
        public Pension? Pension { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        [Display(Name = "Примечание")]
        public string? Note { get; init; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Валидация обязательных документов
            if (Documents != null)
            {
                if (!Documents.ContainsKey(DocumentType.Passport))
                {
                    yield return new ValidationResult("Паспорт обязателен для заполнения", new[] { nameof(Documents) });
                }

                if (!Documents.ContainsKey(DocumentType.MedicalPolicy))
                {
                    yield return new ValidationResult("Полис ОМС обязателен для заполнения", new[] { nameof(Documents) });
                }

                if (!Documents.ContainsKey(DocumentType.Snils))
                {
                    yield return new ValidationResult("СНИЛС обязателен для заполнения", new[] { nameof(Documents) });
                }
            }

            // Валидация дееспособности
            if (IsCapable && Capable == null)
            {
                yield return new ValidationResult("Если пациент дееспособен, необходимо указать информацию о дееспособности", 
                    new[] { nameof(Capable) });
            }

            // Валидация пенсии
            if (ReceivesPension && Pension == null)
            {
                yield return new ValidationResult("Если пациент получает пенсию, необходимо указать информацию о пенсии", 
                    new[] { nameof(Pension) });
            }
        }
    }
}