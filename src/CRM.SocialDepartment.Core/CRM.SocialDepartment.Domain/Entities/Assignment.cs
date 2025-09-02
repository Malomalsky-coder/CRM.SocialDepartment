using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Events;
using CRM.SocialDepartment.Domain.Exceptions;
using DDD.Entities;

namespace CRM.SocialDepartment.Domain.Entities;

public class Assignment : AggregateRoot<Guid>, IArchive, ISoftDelete
{
    public Assignment(string name, DateTime acceptDate, int departmentNumber, string description, DateTime forwardDate, string forwardDepartment, Dictionary<string, string> statusLog, DateTime departmentForwardDate, string assignee, string? note, DateTime creationDate, Guid patientId)
    {
        Name = name;
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
        PatientId = patientId;

        // Генерируем событие создания назначения
        AddDomainEvent(new AssignmentCreatedEvent(this));
    }
    
    public string Name { get; private set; }

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
    public Dictionary<string, string> StatusLog { get; private set; }

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
    public Guid PatientId { get; private set; }

    /// <summary>
    /// Помечен как в архиве (пациент выписан)
    /// </summary>
    public bool IsArchive { get; set; }

    /// <summary>
    /// Помечен как удаленный
    /// </summary>
    public bool SoftDeleted { get; set; }

    public void UpdateDescription(string description)
    {
        Description = description;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateAcceptDate(DateTime acceptDate)
    {
        AcceptDate = acceptDate;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateForwardDate(DateTime forwardDate)
    {
        ForwardDate = forwardDate;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateForwardDepartment(string department)
    {
        ForwardDepartment = department;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateDepartmentNumber(int departmentNumber)
    {
        var oldDepartmentNumber = DepartmentNumber;
        DepartmentNumber = departmentNumber;
        
        // Генерируем специализированное событие смены отделения
        AddDomainEvent(new AssignmentDepartmentChangedEvent(this, oldDepartmentNumber, departmentNumber));
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateDepartmentForwardDate(DateTime forwardDate)
    {
        DepartmentForwardDate = forwardDate;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateAssignee(string assignee)
    {
        Assignee = assignee;
        
        // Генерируем специализированное событие назначения исполнителя
        AddDomainEvent(new AssignmentAssignedEvent(this, assignee));
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    public void UpdateNote(string note)
    {
        if (string.IsNullOrEmpty(note)) throw new ArgumentNullException(nameof(note));
        Note = note;
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }

    /// <summary>
    /// Архивировать назначение
    /// </summary>
    /// <param name="reason">Причина архивирования</param>
    public void Archive(string? reason = null)
    {
        if (IsArchive)
            throw new DomainException("Назначение уже находится в архиве");

        IsArchive = true;
        
        // Генерируем событие архивирования назначения
        AddDomainEvent(new AssignmentArchivedEvent(this, reason));
    }

    /// <summary>
    /// Пометить назначение как удаленное (мягкое удаление)
    /// </summary>
    public void SoftDelete()
    {
        if (SoftDeleted)
            throw new DomainException("Назначение уже помечено как удаленное");

        SoftDeleted = true;
        
        // Генерируем событие удаления назначения
        AddDomainEvent(new AssignmentDeletedEvent(Id, Description, PatientId));
    }

    /// <summary>
    /// Добавить статус в журнал
    /// </summary>
    /// <param name="status">Новый статус</param>
    public void AddStatus(string status)
    {
        if (string.IsNullOrEmpty(status)) 
            throw new ArgumentNullException(nameof(status));

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        StatusLog[timestamp] = status;
        
        // Генерируем событие добавления статуса
        AddDomainEvent(new AssignmentStatusAddedEvent(this, status));
        AddDomainEvent(new AssignmentUpdatedEvent(this));
    }
    
}