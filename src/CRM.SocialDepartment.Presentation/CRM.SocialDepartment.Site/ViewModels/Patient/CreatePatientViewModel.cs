using CRM.SocialDepartment.Domain.Entities.Patients;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    /// <summary>
    /// Модель модального окна для добавления пациента
    /// </summary>
    public class CreatePatientViewModel : PatientViewModelBase, IValidatableObject
    {
        /// <summary>
        /// Дата рождения пациента
        /// </summary>
        [DisplayName("Дата рождения")]
        public required DateTime Birthday { get; init; }

        /// <summary>
        /// Валидация модели
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateTime.Now.ToString("dd.MM.yyyy") == Birthday.ToString("dd.MM.yyyy"))
            {
                yield return new ValidationResult(
                    "Дата рождения не должна быть текущим днем.", [nameof(Birthday)]
                );
            }
            else if (Birthday > DateTime.Now)
            {
                yield return new ValidationResult(
                    "Дата рождения не должна быть в будущем.", [nameof(Birthday)]
                );
            }
        }
    }
}