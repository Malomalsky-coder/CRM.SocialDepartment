using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Site.Models.Patient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    /// <summary>
    /// Модель модального окна для добавления пациента
    /// </summary>
    public class CreatePatientViewModel : IValidatableObject
    {
        /// <summary>
        /// Полное имя пациента
        /// </summary>
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "ФИО не может быть длиннее 100 символов")]
        [DisplayName("ФИО")]
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Дата рождения
        /// </summary>
        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; init; }

        /// <summary>
        /// История болезни
        /// </summary>
        public MedicalHistoryModel MedicalHistory { get; init; } = new();

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        public CitizenshipInfoModel CitizenshipInfo { get; init; } = new();

        /// <summary>
        /// Список документов
        /// </summary>
        public Dictionary<DocumentType, DocumentViewModel> Documents { get; init; } = new();

        /// <summary>
        /// Пациент является дееспособным?
        /// </summary>
        public bool IsCapable { get; init; } = true;

        /// <summary>
        /// Дееспособный
        /// </summary>
        public CapableModel? Capable { get; init; }

        /// <summary>
        /// Получает ли пациент пенсию
        /// </summary>
        public bool ReceivesPension { get; init; } = false;

        /// <summary>
        /// Пенсия
        /// </summary>
        public PensionModel? Pension { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; init; }

        /// <summary>
        /// Валидация модели
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            #region Проверка возраста
            var age = DateTime.Now.Year - Birthday.Year;

            if (age < 0)
            {
                results.Add(new ValidationResult("Дата рождения не может быть в будущем", [nameof(Birthday)]));
            }

            if (Birthday == default)
            {
                results.Add(new ValidationResult("Дата рождения обязательна для заполнения", [nameof(Birthday)]));
            }

            #endregion
            #region Проверка обязательных полей истории болезни
            if (MedicalHistory is not null)
            {
                if (MedicalHistory?.NumberDepartment <= 0)
                {
                    results.Add(new ValidationResult("Номер отделения не может быть меньше или равен 0", ["MedicalHistory.NumberDepartment"]));
                }

                if (MedicalHistory?.HospitalizationType is null || MedicalHistory.HospitalizationType.Value < 0)
                {
                    results.Add(new ValidationResult("Тип госпитализации обязателен для заполнения", ["MedicalHistory.HospitalizationType"]));
                }

                if (string.IsNullOrWhiteSpace(MedicalHistory?.Resolution))
                {
                    results.Add(new ValidationResult("Постановление обязательно для заполнения", ["MedicalHistory.Resolution"]));
                }

                if (string.IsNullOrWhiteSpace(MedicalHistory?.NumberDocument))
                {
                    results.Add(new ValidationResult("Номер истории болезни обязателен для заполнения", ["MedicalHistory.NumberDocument"]));
                }

                if (MedicalHistory?.DateOfReceipt is null || MedicalHistory.DateOfReceipt == default) 
                {
                    results.Add(new ValidationResult("Дата поступления обязателено для заполнения", ["MedicalHistory.DateOfReceipt"]));
                }
            }
            else
            {
                results.Add(new ValidationResult("История болезни обязателеный параметр", ["MedicalHistory"]));
            }
            #endregion
            #region Проверка обязательных полей гражданства
            if (CitizenshipInfo is not null)
            {
                // Проверяем, что гражданство установлено (включая 0 = RussianFederation)
                if (CitizenshipInfo.Citizenship is null)
                {
                    results.Add(new ValidationResult("Гражданство обязательно", ["CitizenshipInfo.Citizenship"]));
                }
            }
            #endregion
            #region Проверка документов
            foreach (var document in Documents)
            {
                if (!string.IsNullOrWhiteSpace(document.Value.Number))
                {
                    var validationError = ValidateDocumentFormat(document.Key, document.Value.Number);

                    if (!string.IsNullOrEmpty(validationError))
                    {
                        results.Add(new ValidationResult(validationError, [$"Documents[{document.Key}]"]));
                    }
                }
            }
            #endregion
            #region Проверка дееспособности
            if (!IsCapable && Capable is null)
            {
                results.Add(new ValidationResult("Для недееспособного пациента необходимо указать информацию о дееспособности", [nameof(Capable)]));
            }

            if (!IsCapable)
            {
                if (Capable is not null && string.IsNullOrEmpty(Capable?.CourtDecision))
                {
                    results.Add(new ValidationResult("Решение суда обязательно для заполнения"));
                }

                if (Capable is not null && Capable?.TrialDate is null)
                {
                    results.Add(new ValidationResult("Дата проведения суда обязательно для заполнения"));
                }

                if (Capable is not null && string.IsNullOrEmpty(Capable?.Guardian))
                {
                    results.Add(new ValidationResult("Опекун обязательно для заполнения"));
                }

                if (Capable is not null && string.IsNullOrEmpty(Capable?.GuardianOrderAppointment))
                {
                    results.Add(new ValidationResult("Распоряжение о назначении опекуна обязательно для заполнения"));
                }
            }
            #endregion
            #region Проверка пенсии
            if (ReceivesPension && Pension == null)
            {
                results.Add(new ValidationResult("Для пациента, получающего пенсию, необходимо указать информацию о пенсии", [nameof(Pension)]));
            }

            if (ReceivesPension)
            {
                if (Pension is not null && Pension?.DisabilityGroup.Value == 0)
                {
                    results.Add(new ValidationResult("Группа инвалидности обязательно для заполнения"));
                }

                if (Pension is not null && Pension?.PensionStartDateTime is null)
                {
                    results.Add(new ValidationResult("Дата установления статуса пенсионера обязательна для заполнения"));
                }

                if (Pension is not null && Pension?.PensionAddress.Value == 0)
                {
                    results.Add(new ValidationResult("Способ получения пенсии обязательно для заполнения"));
                }

                if (Pension is not null && Pension?.SfrBranch == 0 && Pension?.SfrBranch < 1)
                {
                    results.Add(new ValidationResult("Филиал СФР значение не может быть рано 0 или отрицательным"));
                }

                if (Pension is not null && string.IsNullOrEmpty(Pension?.SfrDepartment))
                {
                    results.Add(new ValidationResult("Отделение СФР обязательно для заполнения"));
                }
            }
            #endregion

            return results;
        }

        /// <summary>
        /// Получить отображаемое название документа
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <returns>Название документа</returns>
        private static string? ValidateDocumentFormat(DocumentType documentType, string number)
        {
            return documentType switch
            {
                var dt when dt == DocumentType.Passport         => ValidatePassportFormat(number),
                var dt when dt == DocumentType.MedicalPolicy    => ValidateMedicalPolicyFormat(number),
                var dt when dt == DocumentType.Snils            => ValidateSnilsFormat(number),
                _ => null
            };
        }

        /// <summary>
        /// Валидация формата паспорта
        /// </summary>
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

        /// <summary>
        /// Валидация формата полиса ОМС
        /// </summary>
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

        /// <summary>
        /// Валидация формата СНИЛС
        /// </summary>
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