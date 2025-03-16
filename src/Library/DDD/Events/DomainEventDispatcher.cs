using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDD.Events
{
    /// <summary>
    /// Диспетчер доменных событий, который управляет подпиской и публикацией событий.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DomainEventDispatcher> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DomainEventDispatcher"/>.
        /// </summary>
        /// <param name="mediator">Медиатор для публикации глобальных событий.</param>
        /// <param name="logger">Логгер для регистрации ошибок.</param>
        public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Асинхронно публикует доменные события.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены ассинхронной операции.</param>
        /// <param name="events">Массив доменных событий для публикации.</param>
        public async Task PublishAsync(CancellationToken cancellationToken, params INotification[] events)
        {
            foreach (var @event in events)
            {
                try
                {
                    await _mediator.Publish(@event, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при публикации события: {ex.Message}");
                }
            }
        }
    }
}