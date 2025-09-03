using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Common;
using Microsoft.Extensions.Logging;

namespace CRM.SocialDepartment.Site.Middleware
{
    /// <summary>
    /// Middleware для обновления Claims пользователя с ролями из таблицы userRoles
    /// </summary>
    public class UserRolesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserRolesMiddleware> _logger;

        public UserRolesMiddleware(RequestDelegate next, ILogger<UserRolesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            _logger.LogInformation("UserRolesMiddleware: Начало выполнения для запроса {Path}", context.Request.Path);
            
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    _logger.LogInformation("UserRolesMiddleware: Пользователь аутентифицирован: {UserName}", context.User.Identity.Name);
                    
                    // Логируем все существующие Claims для отладки
                    var allClaims = context.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                    _logger.LogInformation("UserRolesMiddleware: Все Claims пользователя: {Claims}", string.Join(", ", allClaims));
                    
                    // Получаем ID пользователя из Claims
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        _logger.LogInformation("UserRolesMiddleware: ID пользователя: {UserId}", userId);
                        
                        // Проверяем, что UnitOfWork доступен
                        if (unitOfWork == null)
                        {
                            _logger.LogError("UserRolesMiddleware: UnitOfWork равен null!");
                            await _next(context);
                            return;
                        }
                        
                        // Получаем роли пользователя из таблицы userRoles через UnitOfWork
                        var userRoles = await unitOfWork.UserRoles.GetUserRolesAsync(userId);
                        _logger.LogInformation("UserRolesMiddleware: Найдено ролей пользователя: {Count}", userRoles.Count());
                        
                        if (!userRoles.Any())
                        {
                            _logger.LogWarning("UserRolesMiddleware: У пользователя {UserId} нет ролей в таблице userRoles", userId);
                        }
                        
                        // Создаем новый ClaimsIdentity с обновленными ролями
                        var claims = new List<Claim>(context.User.Claims);
                        
                        // Удаляем существующие Claims ролей
                        var existingRoleClaims = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                        claims.RemoveAll(c => c.Type == ClaimTypes.Role);
                        _logger.LogInformation("UserRolesMiddleware: Удалено существующих Claims ролей: {Count}", existingRoleClaims.Count);
                        
                        // Добавляем актуальные роли
                        foreach (var userRole in userRoles)
                        {
                            _logger.LogInformation("UserRolesMiddleware: Обрабатываем userRole: UserId={UserId}, RoleId={RoleId}", userRole.UserId, userRole.RoleId);
                            
                            var role = await unitOfWork.Roles.GetAsync(r => r.GetId() == userRole.RoleId);
                            if (role != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role.Name));
                                _logger.LogInformation("UserRolesMiddleware: Добавлена роль: {RoleName} для пользователя {UserId}", role.Name, userId);
                            }
                            else
                            {
                                _logger.LogWarning("UserRolesMiddleware: Роль с ID {RoleId} не найдена для пользователя {UserId}", userRole.RoleId, userId);
                            }
                        }
                        
                        // Создаем новый ClaimsPrincipal
                        var newIdentity = new ClaimsIdentity(claims, context.User.Identity.AuthenticationType);
                        context.User = new ClaimsPrincipal(newIdentity);
                        
                        _logger.LogInformation("UserRolesMiddleware: Итоговое количество Claims: {Count}", claims.Count);
                        _logger.LogInformation("UserRolesMiddleware: Claims ролей: {Roles}", 
                            string.Join(", ", claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));
                    }
                    else
                    {
                        _logger.LogWarning("UserRolesMiddleware: Не удалось получить ID пользователя из Claims. NameIdentifier: {NameIdentifier}", 
                            userIdClaim?.Value ?? "null");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UserRolesMiddleware: Ошибка при получении ролей пользователя");
                }
            }
            else
            {
                _logger.LogInformation("UserRolesMiddleware: Пользователь не аутентифицирован");
            }

            await _next(context);
        }
    }
}
