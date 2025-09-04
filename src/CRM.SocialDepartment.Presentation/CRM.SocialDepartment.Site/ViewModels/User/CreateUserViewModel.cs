using CRM.SocialDepartment.Site.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MimeKit;

namespace CRM.SocialDepartment.Site.ViewModels.User
{
    public class CreateUserViewModel : ValidationBase, IValidatableObject
    {
        [Required(ErrorMessage = "Логин пользователя обязателен для заполнения")]
        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательно для заполнения")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-mail обязателен для заполнения")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} Должен быть минимум {2} и максимум {1} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Роль в системе обязательна для заполнения")]
        [Display(Name = "Роль в системе")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Должность обязательна для заполнения")]
        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Номер отделения обязателен для заполнения")]
        [Display(Name = "Номер отделения")]
        public string DepartmentNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsValidEmail(Email))
            {
                yield return new ValidationResult("Введите корректный email", new[] { nameof(Email) });
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailboxAddress("", email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
