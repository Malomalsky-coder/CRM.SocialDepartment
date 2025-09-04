using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.Assignment
{
    public partial class EditAssignmentViewModel : IValidatableObject
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название обязательно для заполнения")]
        [DisplayName("Название")]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно для заполнения")]
        [DisplayName("Описание")]
        [StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Создано")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Кому передано")]
        public string? ForwardDepartment { get; set; }

        [Display(Name = "Исполнитель")]
        public string? Assignee { get; set; }

        [Display(Name = "Дата принятия")]
        public DateTime AcceptDate { get; set; }

        [Display(Name = "Дата передачи")]
        public DateTime ForwardDate { get; set; }

        [Display(Name = "Номер отделения")]
        public int DepartmentNumber { get; set; }

        [Display(Name = "Дата передачи в отделение")]
        public DateTime DepartmentForwardDate { get; set; }

        [Display(Name = "Заметка")]
        public string? Note { get; set; }

        [Required]
        public Guid PatientId { get; set; }
    }
}
