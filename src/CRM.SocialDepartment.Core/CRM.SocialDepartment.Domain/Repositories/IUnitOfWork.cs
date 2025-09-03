namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Интерфейс Unit of Work для управления транзакциями и координации репозиториев
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Репозиторий для работы с пациентами
        /// </summary>
        IPatientRepository Patients { get; }

        /// <summary>
        /// Репозиторий для работы с назначениями
        /// </summary>
        IAssignmentRepository Assignments { get; }

        /// <summary>
        /// Репозиторий для работы с пользователями
        /// </summary>
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IRoleRepository Roles { get; }

        /// <summary>
        /// Репозиторий для работы с логами активности пользователей
        /// </summary>
        IUserActivityLogRepository UserActivityLogs { get; }

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Task</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Подтвердить транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Task</returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Отменить транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Task</returns>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Сохранить все изменения
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Количество измененных записей</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить, активна ли транзакция
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Опубликовать все доменные события из агрегатов
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Task</returns>
        Task PublishDomainEventsAsync(CancellationToken cancellationToken = default);
    }
} 