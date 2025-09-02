namespace CRM.SocialDepartment.Site.ViewModels.Assignment;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CreateAssignmentViewModel : IValidatableObject
{
    public int Id { get; set; }
    
    public int DepartmentNumber { get; set; }
    
    [Required(ErrorMessage = "Название обязательно для заполнения")]
    [DisplayName("Название")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Дата принятия обязательна для заполнения")]
    [DisplayName("Дата принятия задания")]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateTime AcceptDate { get; init; }

    [Required(ErrorMessage = "Описание обязательно для заполнения")]
    [DisplayName("Описание")]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Исполнитель обязателен для заполнения")]
    [DisplayName("Исполнитель")]
    public string Assignee { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пациент обязателен для заполнения")]
    [DisplayName("Пациент")]
    public string PatientId { get; set; }

    public string Note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var resultList = new List<ValidationResult>();

        if (string.IsNullOrEmpty(Name))
            resultList.Add(new("Название задачи не может быть пустым", [nameof(Name)]));
        if (string.IsNullOrEmpty(Description))
            resultList.Add(new("Описание не может быть пустым", [nameof(Description)]));
        if (string.IsNullOrEmpty(Assignee))
            resultList.Add(new("Исполнитель не может отсутствовать", [nameof(Assignee)]));

        return resultList;
    }
}