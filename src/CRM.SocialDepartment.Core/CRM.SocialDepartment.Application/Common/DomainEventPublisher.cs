using DDD.Entities;
using DDD.Events;
using MediatR;

namespace CRM.SocialDepartment.Application.Common
{
    /// <summary>
    /// Вспомогательный класс для публикации доменных событий из агрегатов
    /// </summary>
    public static class DomainEventPublisher
    {
        /// <summary>
        /// Опубликовать и очистить доменные события из агрегата
        /// </summary>
        /// <param name="aggregateRoot">Агрегат с событиями</param>
        /// <param name="domainEventDispatcher">Диспетчер событий</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public static async Task PublishAndClearEventsAsync(
            AggregateRoot aggregateRoot, 
            IDomainEventDispatcher? domainEventDispatcher,
            CancellationToken cancellationToken = default)
        {
            if (domainEventDispatcher == null || !aggregateRoot.DomainEvents.Any())
                return;

            var events = aggregateRoot.DomainEvents.ToArray();
            aggregateRoot.ClearDomainEvents();

            await domainEventDispatcher.PublishAsync(cancellationToken, events);
        }

        /// <summary>
        /// Опубликовать и очистить доменные события из агрегата с типизированным ключом
        /// </summary>
        /// <typeparam name="TKey">Тип ключа агрегата</typeparam>
        /// <param name="aggregateRoot">Агрегат с событиями</param>
        /// <param name="domainEventDispatcher">Диспетчер событий</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public static async Task PublishAndClearEventsAsync<TKey>(
            AggregateRoot<TKey> aggregateRoot, 
            IDomainEventDispatcher? domainEventDispatcher,
            CancellationToken cancellationToken = default)
        {
            if (domainEventDispatcher == null || !aggregateRoot.DomainEvents.Any())
                return;

            var events = aggregateRoot.DomainEvents.ToArray();
            aggregateRoot.ClearDomainEvents();

            await domainEventDispatcher.PublishAsync(cancellationToken, events);
        }

        /// <summary>
        /// Опубликовать и очистить доменные события из множества агрегатов
        /// </summary>
        /// <param name="aggregateRoots">Коллекция агрегатов с событиями</param>
        /// <param name="domainEventDispatcher">Диспетчер событий</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public static async Task PublishAndClearEventsAsync(
            IEnumerable<AggregateRoot> aggregateRoots, 
            IDomainEventDispatcher? domainEventDispatcher,
            CancellationToken cancellationToken = default)
        {
            if (domainEventDispatcher == null)
                return;

            var allEvents = new List<INotification>();

            foreach (var aggregateRoot in aggregateRoots)
            {
                if (aggregateRoot.DomainEvents.Any())
                {
                    allEvents.AddRange(aggregateRoot.DomainEvents);
                    aggregateRoot.ClearDomainEvents();
                }
            }

            if (allEvents.Any())
            {
                await domainEventDispatcher.PublishAsync(cancellationToken, allEvents.ToArray());
            }
        }
    }
} 