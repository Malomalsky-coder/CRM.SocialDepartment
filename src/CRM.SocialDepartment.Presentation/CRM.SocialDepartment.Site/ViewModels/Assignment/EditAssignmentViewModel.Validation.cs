using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Assignment
{
    public partial class EditAssignmentViewModel : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var resultList = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Name))
                resultList.Add(new("Название задачи не может быть пустым", new[] { nameof(Name) }));

            if (string.IsNullOrEmpty(Description))
                resultList.Add(new("Описание не может быть пустым", new[] { nameof(Description) }));

            if (string.IsNullOrEmpty(Assignee))
                resultList.Add(new("Исполнитель не может отсутствовать", new[] { nameof(Assignee) }));

            if (PatientId == Guid.Empty)
                resultList.Add(new("Пациент не выбран", new[] { nameof(PatientId) }));

            return resultList;
        }
    }
}
