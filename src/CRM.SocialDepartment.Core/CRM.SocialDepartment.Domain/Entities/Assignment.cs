using CRM.SocialDepartment.Domain.Entities.Patients;
using DDD.Entities;

namespace CRM.SocialDepartment.Domain.Entities;

public class Assignment : AggregateRoot<Guid>, IArchive, ISoftDelete
{
    public Assignment(DateTime acceptDate, int departmentNumber, string description, DateTime forwardDate, string forwardDepartment, Dictionary<DateTime, string> statusLog, DateTime departmentForwardDate, string assignee, string? note, DateTime creationDate, Patient patient)
    {
        AcceptDate = acceptDate;
        DepartmentNumber = departmentNumber;
        Description = description;
        ForwardDate = forwardDate;
        ForwardDepartment = forwardDepartment;
        StatusLog = statusLog;
        DepartmentForwardDate = departmentForwardDate;
        Assignee = assignee;
        Note = note;
        CreationDate = creationDate;
        Patient = patient;
    }

    /// <summary>
    /// Дата приема заявки от отделения
    /// </summary>
    public DateTime AcceptDate { get; private set; }

    /// <summary>
    /// Номер отделения
    /// </summary>
    public int DepartmentNumber { get; private set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Дата направления
    /// </summary>
    public DateTime ForwardDate { get; private set; }

    /// <summary>
    /// Куда направили документы
    /// </summary>
    public string ForwardDepartment { get; private set; } /* TODO: replace string with Department datatype */

    /// <summary>
    /// Что сделано
    /// </summary>
    public Dictionary<DateTime, string> StatusLog { get; private set; }

    /// <summary>
    /// Дата передачи в отделение
    /// </summary>
    public DateTime DepartmentForwardDate { get; private set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public string Assignee { get; private set; } /* TODO: replace string with User datatype or something like that */

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    /// Дата создания задачи
    /// </summary>
    public DateTime CreationDate { get; private set; }

    /// <summary>
    /// Пациент
    /// </summary>
    public Patient Patient { get; private set; }

    /// <summary>
    /// Помечен как в архиве (пациент выписан)
    /// </summary>
    public bool IsArchive { get; set; }

    /// <summary>
    /// Помечен как удаленный
    /// </summary>
    public bool SoftDeleted { get; set; }
}