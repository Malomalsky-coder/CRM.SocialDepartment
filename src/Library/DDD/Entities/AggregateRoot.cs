using DDD.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Entities
{
    [Serializable]
    public abstract class AggregateRoot : Entity, IAggregateRoot, IHasConcurrencyStamp
    {
        public virtual string ConcurrencyStamp { get; set; }

        private readonly List<INotification> _domainEvents = new List<INotification>();

        protected AggregateRoot()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Получить список доменных событий
        /// </summary>
        public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Добавить доменное событие
        /// </summary>
        /// <param name="domainEvent">Доменное событие</param>
        protected void AddDomainEvent(INotification domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Удалить доменное событие
        /// </summary>
        /// <param name="domainEvent">Доменное событие</param>
        protected void RemoveDomainEvent(INotification domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        /// <summary>
        /// Очистить все доменные события
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    [Serializable]
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IHasConcurrencyStamp
    {
        public virtual string ConcurrencyStamp { get; set; }

        private readonly List<INotification> _domainEvents = new List<INotification>();

        protected AggregateRoot()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        protected AggregateRoot(TKey id)
            : base(id)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Получить список доменных событий
        /// </summary>
        public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Добавить доменное событие
        /// </summary>
        /// <param name="domainEvent">Доменное событие</param>
        protected void AddDomainEvent(INotification domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Удалить доменное событие
        /// </summary>
        /// <param name="domainEvent">Доменное событие</param>
        protected void RemoveDomainEvent(INotification domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        /// <summary>
        /// Очистить все доменные события
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}