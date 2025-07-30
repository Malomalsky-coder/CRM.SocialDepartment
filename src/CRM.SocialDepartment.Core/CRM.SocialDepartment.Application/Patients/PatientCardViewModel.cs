using System.ComponentModel.DataAnnotations;

namespace CRM.SocialDepartment.Application.Patients
{
    public class PatientCardViewModel
    {
        public Guid PatientId { get; set; }
        
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Дата рождения")]
        public DateTime Birthday { get; set; }
        
        [Display(Name = "Гражданство")]
        public string Citizenship { get; set; } = string.Empty;
        
        [Display(Name = "Страна")]
        public string Country { get; set; } = string.Empty;
        
        [Display(Name = "Номер отделения")]
        public string NumberDepartment { get; set; } = string.Empty;
        
        [Display(Name = "Примечание")]
        public string? Note { get; set; }
        
        [Display(Name = "Не зарегистрирован")]
        public bool NoRegistration { get; set; }
        
        [Display(Name = "Ранняя регистрация")]
        public int EarlyRegistration { get; set; }
        
        [Display(Name = "Место регистрации")]
        public string? Registration { get; set; }
        
        [Display(Name = "Прикрепленные документы")]
        public string? DocumentAttached { get; set; }
        
        [Display(Name = "Паспортные данные")]
        public string? Passport { get; set; }
        
        [Display(Name = "СНИЛС")]
        public string? Snils { get; set; }
        
        [Display(Name = "Медицинский полис")]
        public string? MedicalPolicy { get; set; }
        
        [Display(Name = "Дееспособный")]
        public bool IsCapable { get; set; }
        
        [Display(Name = "Решение суда")]
        public string? CourtDecision { get; set; }
        
        [Display(Name = "Дата проведения суда")]
        public DateTime? TrialDate { get; set; }
        
        [Display(Name = "Опекун")]
        public string? Guardian { get; set; }
        
        [Display(Name = "Распоряжение о назначении опекуна")]
        public string? GuardianOrderAppointment { get; set; }
        
        [Display(Name = "Получает пенсию")]
        public bool PensionIsActive { get; set; }
        
        [Display(Name = "Группа инвалидности")]
        public int DisabilityGroup { get; set; }
        
        [Display(Name = "Дата начала пенсии")]
        public DateTime? PensionStartDateTime { get; set; }
        
        [Display(Name = "Способ получения пенсии")]
        public int PensionAddress { get; set; }
        
        [Display(Name = "Филиал СФР")]
        public string? SfrBranch { get; set; }
        
        [Display(Name = "Отделение СФР")]
        public string? SfrDepartment { get; set; }
        
        [Display(Name = "РСД")]
        public string? Rsd { get; set; }
        
        [Display(Name = "В архиве")]
        public bool IsArchive { get; set; }
        
        // Архивная информация
        public PatientArchiveViewModel? Archive { get; set; }
        
        // История болезней
        public List<MedicalHistoryViewModel> HistoryOfIllnesses { get; set; } = new();
    }
    
    public class PatientArchiveViewModel
    {
        public int Status { get; set; }
        public string? Note { get; set; }
    }
    
    public class MedicalHistoryViewModel
    {
        public Guid Id { get; set; }
        
        [Display(Name = "Номер документа")]
        public string NumberDocument { get; set; } = string.Empty;
        
        [Display(Name = "Дата поступления")]
        public DateTime DateOfReceipt { get; set; }
        
        [Display(Name = "Дата выписки")]
        public DateTime? DateOfDischarge { get; set; }
        
        [Display(Name = "Примечание")]
        public string? Note { get; set; }
    }
} 