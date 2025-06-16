#nullable disable
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace CRM.SocialDepartment.Site.ViewModels.Patient
{
    /// <summary>
    /// Базовый класс ViewModel для Patient
    /// </summary>
    public abstract class PatientViewModelBase
    {
        /// <summary>
        /// Полное имя пациента
        /// </summary>
        [DisplayName("ФИО")]
        public string FullName { get; init; }

        /// <summary>
        /// История болезни
        /// </summary>
        [DisplayName("История болезни")]
        public MedicalHistory MedicalHistory { get; init; }

        /// <summary>
        /// Информация о гражданстве
        /// </summary>
        [DisplayName("Информация о гражданстве")]
        public CitizenshipInfo CitizenshipInfo { get; init; }

        /// <summary>
        /// Список документов
        /// </summary>
        [DisplayName("Список документов")]
        public Dictionary<DocumentType, DocumentViewModel> Documents { get; init; }

        /// <summary>
        /// Пациент является дееспособным?
        /// </summary>
        [DisplayName("Дееспособность")]
        public bool IsCapable { get; init; }

        /// <summary>
        /// Дееспособный
        /// </summary>
        [DisplayName("Информация о дееспособности")]
        public Capable? Capable { get; init; }

        /// <summary>
        /// Получает ли пациент пенсию
        /// </summary>
        [DisplayName("Получение пенсии")]
        public bool ReceivesPension { get; init; }

        /// <summary>
        /// Пенсия
        /// </summary>
        [DisplayName("Информация о пенсии")]
        public Pension? Pension { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        [DisplayName("Примечание")]
        public string? Note { get; init; }
    }

    /// <summary>
    /// История болезни
    /// </summary>
    public class MedicalHistory
    {
        /// <summary>
        /// Номер отделения
        /// </summary>
        [DisplayName("Номер отделения")]
        public sbyte NumberDepartment { get; init; }

        /// <summary>
        /// Тип госпитализации
        /// </summary>
        [DisplayName("Тип госпитализации")]
        public HospitalizationType HospitalizationType { get; init; }

        /// <summary>
        /// Постановление
        /// </summary>
        [DisplayName("Постановление")]
        public string Resolution { get; init; }

        /// <summary>
        /// Номер истории болезни
        /// </summary>
        [DisplayName("Номер истории болезни")]
        public string NumberDocument { get; init; }

        /// <summary>
        /// Дата поступления
        /// </summary>
        [DisplayName("Дата поступления")]
        public DateTime DateOfReceipt { get; init; }

        /// <summary>
        /// Примечание
        /// </summary>
        [DisplayName("Примечание")]
        public string? Note { get; init; }
    }

    /// <summary>
    /// Информация о гражданстве
    /// </summary>
    public class CitizenshipInfo
    {
        [BindNever]
        [DisplayName("Список гражданств")]
        public string[] Citizenships { get; init; }

        /// <summary>
        /// Гражданство
        /// </summary>
        [DisplayName("Гражданство")]
        public CitizenshipType Citizenship { get; init; }

        /// <summary>
        /// Страна
        /// </summary>
        [DisplayName("Страна")]
        public string? Country { get; init; }

        /// <summary>
        /// Место регистрации
        /// </summary>
        [DisplayName("Место регистрации")]
        public string? Registration { get; init; }

        /// <summary>
        /// Нет регистрации, бомж
        /// </summary>
        [DisplayName("Нет регистрации")]
        public bool NotRegistered { get; init; }

        /// <summary>
        /// Ранняя регистрация
        /// </summary>
        [DisplayName("Ранняя регистрация")]
        public City? EarlyRegistration { get; init; }

        /// <summary>
        /// Место рождения
        /// </summary>
        [DisplayName("Место рождения")]
        public string? PlaceOfBirth { get; init; }

        /// <summary>
        /// Имеющиеся документы
        /// </summary>
        [DisplayName("Имеющиеся документы")]
        public string DocumentAttached { get; init; }
    }

    /// <summary>
    /// Дееспособнный
    /// </summary>
    public class Capable
    {
        /// <summary>
        /// Решение суда
        /// </summary>
        [DisplayName("Решение суда")]
        public string CourtDecision { get; init; }

        /// <summary>
        /// Дата проведения суда
        /// </summary>
        [DisplayName("Дата проведения суда")]
        public DateTime? TrialDate { get; init; }

        /// <summary>
        /// Опекун
        /// </summary>
        [DisplayName("Опекун")]
        public string Guardian { get; init; }

        /// <summary>
        /// Распоряжение о назначение опекуна
        /// </summary>
        [DisplayName("Распоряжение о назначении опекуна")]
        public string GuardianOrderAppointment { get; init; }
    }

    /// <summary>
    /// Пенсия
    /// </summary>
    public class Pension
    {
        /// <summary>
        /// Группа инвалидности
        /// </summary>
        [DisplayName("Группа инвалидности")]
        public DisabilityGroup DisabilityGroup { get; init; }

        /// <summary>
        /// С какого числа установлен статус пенсионера
        /// </summary>
        [DisplayName("Дата установления статуса пенсионера")]
        public DateTime? PensionStartDateTime { get; init; }

        /// <summary>
        /// Способ получения пенсии
        /// </summary>
        [DisplayName("Способ получения пенсии")]
        public PensionAddress PensionAddress { get; init; }

        /// <summary>
        /// Филиал СФР
        /// </summary>
        [DisplayName("Филиал СФР")]
        public int SfrBranch { get; init; }

        /// <summary>
        /// Отделение СФР
        /// </summary>
        [DisplayName("Отделение СФР")]
        public string SfrDepartment { get; init; }

        /// <summary>
        /// РСД
        /// </summary>
        [DisplayName("РСД")]
        public string? Rsd { get; init; }
    }
}
