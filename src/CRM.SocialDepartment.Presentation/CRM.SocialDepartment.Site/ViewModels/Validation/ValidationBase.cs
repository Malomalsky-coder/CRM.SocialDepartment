using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Validation
{
    public abstract class ValidationBase
    {
        /// <summary>
        /// Проверяет, что строка не пустая и не состоит только из пробелов
        /// </summary>
        protected static ValidationResult? ValidateRequiredString(string? value, string displayName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return new ValidationResult($"{displayName} не может быть пустым");
            }
            return null;
        }

        /// <summary>
        /// Проверяет, что дата не в будущем
        /// </summary>
        protected static ValidationResult? ValidateNotFutureDate(DateTime? date, string displayName)
        {
            if (date.HasValue && date.Value > DateTime.Now)
            {
                return new ValidationResult($"{displayName} не может быть в будущем");
            }
            return null;
        }

        /// <summary>
        /// Проверяет, что дата не в прошлом
        /// </summary>
        protected static ValidationResult? ValidateNotPastDate(DateTime? date, string displayName)
        {
            if (date.HasValue && date.Value < DateTime.Now.Date)
            {
                return new ValidationResult($"{displayName} не может быть в прошлом");
            }
            return null;
        }
    }
} 