using MediatR;
using System;

namespace DDD.Events
{
    /// <summary>
    /// Доменное событие.
    /// </summary>
    [Serializable]
    public abstract class DomainEvent : INotification
    {
    }
}