using CRM.SocialDepartment.Domain.Entities;
using DDD.Events;

namespace CRM.SocialDepartment.Domain.Events
{
    /// <summary>
    /// Событие создания нового назначения
    /// </summary>
    public class AssignmentCreatedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public DateTime CreatedAt { get; }

        public AssignmentCreatedEvent(Assignment assignment)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            CreatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие обновления назначения
    /// </summary>
    public class AssignmentUpdatedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public DateTime UpdatedAt { get; }

        public AssignmentUpdatedEvent(Assignment assignment)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие удаления назначения
    /// </summary>
    public class AssignmentDeletedEvent : DomainEvent
    {
        public Guid AssignmentId { get; }
        public string Description { get; }
        public Guid PatientId { get; }
        public DateTime DeletedAt { get; }

        public AssignmentDeletedEvent(Guid assignmentId, string description, Guid patientId)
        {
            AssignmentId = assignmentId;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PatientId = patientId;
            DeletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие назначения задачи исполнителю
    /// </summary>
    public class AssignmentAssignedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public string Assignee { get; }
        public DateTime AssignedAt { get; }

        public AssignmentAssignedEvent(Assignment assignment, string assignee)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            Assignee = assignee ?? throw new ArgumentNullException(nameof(assignee));
            AssignedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие перевода назначения в архив
    /// </summary>
    public class AssignmentArchivedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public DateTime ArchivedAt { get; }
        public string? Reason { get; }

        public AssignmentArchivedEvent(Assignment assignment, string? reason = null)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            ArchivedAt = DateTime.UtcNow;
            Reason = reason;
        }
    }

    /// <summary>
    /// Событие смены отделения для назначения
    /// </summary>
    public class AssignmentDepartmentChangedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public int OldDepartmentNumber { get; }
        public int NewDepartmentNumber { get; }
        public DateTime ChangedAt { get; }

        public AssignmentDepartmentChangedEvent(Assignment assignment, int oldDepartmentNumber, int newDepartmentNumber)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            OldDepartmentNumber = oldDepartmentNumber;
            NewDepartmentNumber = newDepartmentNumber;
            ChangedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие добавления статуса к назначению
    /// </summary>
    public class AssignmentStatusAddedEvent : DomainEvent
    {
        public Assignment Assignment { get; }
        public string Status { get; }
        public DateTime StatusAddedAt { get; }

        public AssignmentStatusAddedEvent(Assignment assignment, string status)
        {
            Assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
            Status = status ?? throw new ArgumentNullException(nameof(status));
            StatusAddedAt = DateTime.UtcNow;
        }
    }
} 