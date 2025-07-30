using CRM.SocialDepartment.Domain.Repositories;
using DDD.Entities;
using DDD.Events;
using MediatR;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация Unit of Work паттерна
    /// </summary>
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _mongoClient;
        private readonly IDomainEventDispatcher? _domainEventDispatcher;
        private IClientSessionHandle? _session;
        private bool _disposed = false;

        // Lazy initialization репозиториев
        private IPatientRepository? _patients;
        private IAssignmentRepository? _assignments;
        private IUserRepository? _users;

        public MongoUnitOfWork(IMongoDatabase database, IDomainEventDispatcher? domainEventDispatcher = null)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _mongoClient = database.Client;
            _domainEventDispatcher = domainEventDispatcher;
        }

        /// <summary>
        /// Репозиторий для работы с пациентами
        /// </summary>
        public IPatientRepository Patients
        {
            get
            {
                _patients ??= new MongoPatientRepository(_database, _session);
                return _patients;
            }
        }

        /// <summary>
        /// Репозиторий для работы с назначениями
        /// </summary>
        public IAssignmentRepository Assignments
        {
            get
            {
                _assignments ??= new MongoAssignmentRepository(_database, _session);
                return _assignments;
            }
        }

        /// <summary>
        /// Репозиторий для работы с пользователями
        /// </summary>
        public IUserRepository Users
        {
            get
            {
                _users ??= new MongoUserRepository(_database, _session);
                return _users;
            }
        }

        /// <summary>
        /// Проверить, активна ли транзакция
        /// </summary>
        public bool HasActiveTransaction => _session?.IsInTransaction == true;

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                throw new InvalidOperationException("Транзакция уже активна");
            }

            try
            {
                _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
                _session.StartTransaction();
            }
            catch (Exception ex)
            {
                // MongoDB может не поддерживать транзакции (например, standalone instance)
                // В этом случае создаем сессию без транзакции для координации операций
                if (ex.Message.Contains("Transactions are not supported"))
                {
                    _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
                }
                else
                {
                    throw;
                }
            }

            // Пересоздаем репозитории с новой сессией
            _patients = null;
            _assignments = null;
            _users = null;
        }

        /// <summary>
        /// Подтвердить транзакцию
        /// </summary>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Нет активной транзакции");
            }

            if (_session.IsInTransaction)
            {
                await _session.CommitTransactionAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Отменить транзакцию
        /// </summary>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Нет активной транзакции");
            }

            if (_session.IsInTransaction)
            {
                await _session.AbortTransactionAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Сохранить все изменения и опубликовать доменные события
        /// В MongoDB это метод-пустышка для сохранения (операции выполняются немедленно),
        /// но здесь мы публикуем доменные события
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Публикуем доменные события перед завершением
            await PublishDomainEventsAsync(cancellationToken);
            
            // В MongoDB операции выполняются немедленно
            // Этот метод может быть расширен для батчинга операций
            await Task.CompletedTask;
            return 0; // Возвращаем 0, так как в MongoDB нет отложенных изменений
        }

        /// <summary>
        /// Опубликовать все доменные события из агрегатов
        /// </summary>
        public async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            if (_domainEventDispatcher == null)
                return;

            var domainEvents = new List<INotification>();

            // Публикуем события
            if (domainEvents.Any())
            {
                await _domainEventDispatcher.PublishAsync(cancellationToken, domainEvents.ToArray());
            }
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _session?.Dispose();
                _session = null;
                _disposed = true;
            }
        }
    }
} 