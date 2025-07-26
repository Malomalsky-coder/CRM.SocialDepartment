using CRM.SocialDepartment.Domain.Events;
using DDD.Events;
using Microsoft.Extensions.Logging;

namespace CRM.SocialDepartment.Application.Handlers.EventHandlers
{
    /// <summary>
    /// Обработчик события создания пациента
    /// </summary>
    public class PatientCreatedEventHandler : IDomainEventHandler<PatientCreatedEvent>
    {
        private readonly ILogger<PatientCreatedEventHandler> _logger;

        public PatientCreatedEventHandler(ILogger<PatientCreatedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Создан новый пациент: {PatientId} - {PatientName} в {CreatedAt}", 
                notification.Patient.Id, 
                notification.Patient.FullName, 
                notification.CreatedAt);

            // Здесь можно добавить дополнительную логику:
            // - Отправка уведомлений
            // - Логирование в аудит
            // - Интеграция с внешними системами
            // - Создание задач для персонала

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Обработчик события обновления пациента
    /// </summary>
    public class PatientUpdatedEventHandler : IDomainEventHandler<PatientUpdatedEvent>
    {
        private readonly ILogger<PatientUpdatedEventHandler> _logger;

        public PatientUpdatedEventHandler(ILogger<PatientUpdatedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обновлены данные пациента: {PatientId} - {PatientName} в {UpdatedAt}", 
                notification.Patient.Id, 
                notification.Patient.FullName, 
                notification.UpdatedAt);

            // Логика для обработки обновлений:
            // - Синхронизация с внешними системами
            // - Уведомление заинтересованных лиц
            // - Обновление кэша

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Обработчик события удаления пациента
    /// </summary>
    public class PatientDeletedEventHandler : IDomainEventHandler<PatientDeletedEvent>
    {
        private readonly ILogger<PatientDeletedEventHandler> _logger;

        public PatientDeletedEventHandler(ILogger<PatientDeletedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Удален пациент: {PatientId} - {PatientName} в {DeletedAt}", 
                notification.PatientId, 
                notification.PatientName, 
                notification.DeletedAt);

            // Логика для обработки удаления:
            // - Аудит удаления
            // - Проверка связанных записей
            // - Уведомление администраторов
            // - Очистка связанных данных

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Обработчик события архивирования пациента
    /// </summary>
    public class PatientArchivedEventHandler : IDomainEventHandler<PatientArchivedEvent>
    {
        private readonly ILogger<PatientArchivedEventHandler> _logger;

        public PatientArchivedEventHandler(ILogger<PatientArchivedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientArchivedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Пациент {PatientId} - {PatientName} архивирован в {ArchivedAt}. Причина: {Reason}", 
                notification.Patient.Id, 
                notification.Patient.FullName, 
                notification.ArchivedAt,
                notification.Reason ?? "Не указана");

            // Логика архивирования:
            // - Закрытие активных задач
            // - Уведомление персонала
            // - Перенос в архивную систему
            // - Создание отчетов о выписке

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Обработчик события восстановления пациента из архива
    /// </summary>
    public class PatientUnarchivedEventHandler : IDomainEventHandler<PatientUnarchivedEvent>
    {
        private readonly ILogger<PatientUnarchivedEventHandler> _logger;

        public PatientUnarchivedEventHandler(ILogger<PatientUnarchivedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientUnarchivedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Пациент {PatientId} - {PatientName} восстановлен из архива в {UnarchivedAt}", 
                notification.Patient.Id, 
                notification.Patient.FullName, 
                notification.UnarchivedAt);

            // Логика восстановления:
            // - Восстановление доступа
            // - Уведомление персонала
            // - Проверка актуальности данных

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Обработчик события добавления документа к пациенту
    /// </summary>
    public class PatientDocumentAddedEventHandler : IDomainEventHandler<PatientDocumentAddedEvent>
    {
        private readonly ILogger<PatientDocumentAddedEventHandler> _logger;

        public PatientDocumentAddedEventHandler(ILogger<PatientDocumentAddedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PatientDocumentAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("К пациенту {PatientId} добавлен документ {DocumentType}: {DocumentNumber} в {AddedAt}", 
                notification.PatientId, 
                notification.DocumentType, 
                notification.DocumentNumber,
                notification.AddedAt);

            // Логика обработки документов:
            // - Валидация документа
            // - Создание задач на проверку
            // - Интеграция с документооборотом
            // - Уведомление ответственных лиц

            await Task.CompletedTask;
        }
    }
} 