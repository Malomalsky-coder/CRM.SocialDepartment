using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DDD.Events
{
    public interface IDomainEventDispatcher
    {
        public Task PublishAsync(CancellationToken cancellationToken, params INotification[] @event);
    }
}