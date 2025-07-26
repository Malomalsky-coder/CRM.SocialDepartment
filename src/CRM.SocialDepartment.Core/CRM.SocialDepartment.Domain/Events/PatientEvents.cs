using CRM.SocialDepartment.Domain.Entities.Patients;
using DDD.Events;

namespace CRM.SocialDepartment.Domain.Events
{
    /// <summary>
    /// Событие создания нового пациента
    /// </summary>
    public class PatientCreatedEvent : DomainEvent
    {
        public Patient Patient { get; }
        public DateTime CreatedAt { get; }

        public PatientCreatedEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            CreatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие обновления данных пациента
    /// </summary>
    public class PatientUpdatedEvent : DomainEvent
    {
        public Patient Patient { get; }
        public DateTime UpdatedAt { get; }

        public PatientUpdatedEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие удаления пациента
    /// </summary>
    public class PatientDeletedEvent : DomainEvent
    {
        public Guid PatientId { get; }
        public string PatientName { get; }
        public DateTime DeletedAt { get; }

        public PatientDeletedEvent(Guid patientId, string patientName)
        {
            PatientId = patientId;
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
            DeletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие архивирования пациента
    /// </summary>
    public class PatientArchivedEvent : DomainEvent
    {
        public Patient Patient { get; }
        public DateTime ArchivedAt { get; }
        public string? Reason { get; }

        public PatientArchivedEvent(Patient patient, string? reason = null)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            ArchivedAt = DateTime.UtcNow;
            Reason = reason;
        }
    }

    /// <summary>
    /// Событие восстановления пациента из архива
    /// </summary>
    public class PatientUnarchivedEvent : DomainEvent
    {
        public Patient Patient { get; }
        public DateTime UnarchivedAt { get; }

        public PatientUnarchivedEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            UnarchivedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Событие добавления документа к пациенту
    /// </summary>
    public class PatientDocumentAddedEvent : DomainEvent
    {
        public Guid PatientId { get; }
        public string DocumentType { get; }
        public string DocumentNumber { get; }
        public DateTime AddedAt { get; }

        public PatientDocumentAddedEvent(Guid patientId, string documentType, string documentNumber)
        {
            PatientId = patientId;
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            AddedAt = DateTime.UtcNow;
        }
    }
} 