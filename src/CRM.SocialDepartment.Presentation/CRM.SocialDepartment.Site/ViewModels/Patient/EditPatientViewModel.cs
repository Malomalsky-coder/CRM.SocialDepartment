using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.Helpers;
using CRM.SocialDepartment.Site.Models.Patient;
using CRM.SocialDepartment.Site.ViewModels.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(100, ErrorMessage = "ФИО не может быть длиннее 100 символов")]
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// История болезни
        /// </summary>
        [Required(ErrorMessage = "История болезни обязательна для заполнения")]
        public MedicalHistoryModel MedicalHistory { get; init; } = new();

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        [Required(ErrorMessage = "Информация о гражданстве обязательна для заполнения")]
        public CitizenshipInfoModel CitizenshipInfo { get; init; } = new();

        /// <summary>
        /// Список документов
        /// </summary>
        [Display(Name = "Список документов")]
        public Dictionary<DocumentType, DocumentViewModel> Documents { get; init; } = new();

        /// <summary>
        /// Пациент является дееспособным?
        /// </summary>
        [Display(Name = "Дееспособность")]
        public bool IsCapable { get; init; }

        /// <summary>
        /// Дееспособный
        /// </summary>
        [Display(Name = "Информация о дееспособности")]
        public CapableModel? Capable { get; init; }

        /// <summary>
        /// Получает ли пациент пенсию
        /// </summary>
        [Display(Name = "Получение пенсии")]
        public bool ReceivesPension { get; init; }

        /// <summary>
        /// Пенсия
        /// </summary>
        [Display(Name = "Информация о пенсии")]
        public PensionModel? Pension { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        [Display(Name = "Примечание")]
        public string? Note { get; init; }

        /// <summary>
        /// Дата рождения пациента
        /// </summary>
        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; }

        [DisplayName("Несовершеннолетний")]
        public bool IsChildren { get; private set; } //todo: уточнить про модификатор private set, чтобы было автообновление, без ручного обновления

        // Вспомогательные флаги для восстановления состояния формы редактирования ///////////////////////////////////////////////////

        [BindNever]
        public string NoRegistrationIsEnable { get; set; }

        [BindNever]
        public string CountryIsEnable { get; set; }

        [BindNever]
        public string RegistrationIsEnable { get; set; }

        [BindNever]
        public string EarlyRegistrationIsEnable { get; set; }

        [BindNever]
        public string LbgIsEnable { get; set; }

        [BindNever]
        public string DocumentIsEnable { get; set; }

        [BindNever]
        public string CapableIsEnable { get; set; }

        [BindNever]
        public string PensionFieldsetIsEnable { get; set; }

        [BindNever]
        public string PensionStartDateTimeIsEnable { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Проверка документов
            foreach (var document in Documents)
            {
                if (!string.IsNullOrWhiteSpace(document.Value.Number))
                {
                    var validationError = ValidateDocumentFormat(document.Key, document.Value.Number);
                    if (!string.IsNullOrEmpty(validationError))
                    {
                        results.Add(new ValidationResult(validationError, new[] { $"Documents[{document.Key}]" }));
                    }
                }
            }

            return results;
        }

        private static string? ValidateDocumentFormat(DocumentType documentType, string number)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport => ValidatePassportFormat(number),
                var dt when dt == DocumentType.MedicalPolicy => ValidateMedicalPolicyFormat(number),
                var dt when dt == DocumentType.Snils => ValidateSnilsFormat(number),
                _ => null
            };
        }

        private static string? ValidatePassportFormat(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return null;

            var cleanNumber = number.Replace(" ", "");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanNumber, @"^\d{10}$"))
            {
                return "Номер паспорта должен содержать 10 цифр в формате: 1234 567890";
            }

            return null;
        }

        private static string? ValidateMedicalPolicyFormat(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return null;

            var cleanNumber = number.Replace(" ", "");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanNumber, @"^\d{16}$"))
            {
                return "Номер полиса ОМС должен содержать 16 цифр";
            }

            return null;
        }

        private static string? ValidateSnilsFormat(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return null;

            if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\d{3}-\d{3}-\d{3}\s\d{2}$"))
            {
                return "Номер СНИЛС должен быть в формате: 123-456-789 01";
            }

            return null;
        }
    }
}