using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Roles;
using CRM.SocialDepartment.Application.Users;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.SocialDepartment.Application.Common
{
    /// <summary>
    /// Hosted Service для инициализации администратора при запуске приложения
    /// </summary>
    public class AdminInitializationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdminInitializationHostedService> _logger;

        public AdminInitializationHostedService(
            IServiceProvider serviceProvider,
            ILogger<AdminInitializationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Запуск AdminInitializationHostedService");

            try
            {
                // Создаем scope для получения сервисов
                using var scope = _serviceProvider.CreateScope();
                var adminInitService = scope.ServiceProvider.GetRequiredService<AdminInitializationService>();

                // Выполняем инициализацию
                await adminInitService.InitializeAdminAsync();
                
                _logger.LogInformation("Инициализация администратора завершена успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации администратора");
                // Не пробрасываем исключение, чтобы приложение могло запуститься
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка AdminInitializationHostedService");
            return Task.CompletedTask;
        }
    }
}
