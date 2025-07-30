using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Application.DTOs
{
    public class ActivePatientDTO
    {
        public Guid Id { get; set; }
        
        [Display(Name = "Тип госпитализации")]
        public string HospitalizationType { get; set; } = string.Empty;
        
        [Display(Name = "Постановление")]
        public string? CourtDecision { get; set; }
        
        [Display(Name = "Номер истории болезни")]
        public string NumberDocument { get; set; } = string.Empty;
        
        [Display(Name = "Дата поступления")]
        public DateTime DateOfReceipt { get; set; }
        
        [Display(Name = "Отделение")]
        public string Department { get; set; } = string.Empty;
        
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Дата рождения")]
        public DateTime Birthday { get; set; }
        
        [Display(Name = "Несовершеннолетний")]
        public bool IsChildren { get; set; }
        
        [Display(Name = "Гражданство")]
        public string Citizenship { get; set; } = string.Empty;
        
        [Display(Name = "Страна")]
        public string? Country { get; set; }
        
        [Display(Name = "Место регистрации")]
        public string? Registration { get; set; }
        
        [Display(Name = "БОМЖ")]
        public bool IsHomeless { get; set; }
        
        [Display(Name = "Ранняя регистрация")]
        public string? EarlyRegistration { get; set; }
        
        [Display(Name = "Место рождения")]
        public string? PlaceOfBirth { get; set; }
        
        [Display(Name = "Дееспособный")]
        public bool IsCapable { get; set; }
        
        [Display(Name = "Получает пенсию")]
        public bool ReceivesPension { get; set; }
        
        [Display(Name = "Группа инвалидности")]
        public string? DisabilityGroup { get; set; }
        
        [Display(Name = "Примечание")]
        public string? Note { get; set; }
    }
} 