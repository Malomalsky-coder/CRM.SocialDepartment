using CRM.SocialDepartment.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Site.ViewModels.UserActivityLog
{
    /// <summary>
    /// ViewModel для фильтрации логов активности
    /// </summary>
    public class UserActivityLogFilterViewModel
    {
        [Display(Name = "Пользователь")]
        public Guid? UserId { get; set; }

        [Display(Name = "Тип действия")]
        public UserActivityType? ActivityType { get; set; }

        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Тип сущности")]
        public string? EntityType { get; set; }

        [Display(Name = "Страница")]
        public int Page { get; set; } = 1;

        [Display(Name = "Размер страницы")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Список типов действий для выпадающего списка
        /// </summary>
        public static Dictionary<UserActivityType, string> ActivityTypeOptions => new()
        {
            { UserActivityType.Login, "Авторизация" },
            { UserActivityType.Logout, "Выход" },
            { UserActivityType.DataRequest, "Запрос данных" },
            { UserActivityType.Create, "Создание" },
            { UserActivityType.Update, "Редактирование" },
            { UserActivityType.Delete, "Удаление" }
        };

        /// <summary>
        /// Список типов сущностей для выпадающего списка
        /// </summary>
        public static Dictionary<string, string> EntityTypeOptions => new()
        {
            { "Patient", "Пациент" },
            { "Assignment", "Назначение" },
            { "User", "Пользователь" },
            { "MedicalHistory", "Медицинская карта" },
            { "CitizenshipInfo", "Гражданство" },
            { "Pension", "Пенсия" },
            { "Capable", "Дееспособность" }
        };
    }
}
