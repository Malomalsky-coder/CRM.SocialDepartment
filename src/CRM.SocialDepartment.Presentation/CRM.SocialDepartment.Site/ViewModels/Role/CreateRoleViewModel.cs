using CRM.SocialDepartment.Site.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Role
{
    public class CreateRoleViewModel : ValidationBase, IValidatableObject
    {
        [Required(ErrorMessage = "Название роли обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название роли не может быть длиннее 100 символов")]
        [Display(Name = "Название роли")]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Название роли не может быть пустым", new[] { nameof(Name) });
            }
        }
    }
}
