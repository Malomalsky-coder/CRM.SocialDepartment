using MediatR;

namespace DDD.Events
{
    /// <summary>
    /// Представляет обработчик глобальных доменных событий.
    /// </summary>
    /// <typeparam name="TEvent">Тип доменного события.</typeparam>
    public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : INotification
    {
    }
}