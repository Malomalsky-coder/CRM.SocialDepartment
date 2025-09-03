using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Roles;
using CRM.SocialDepartment.Application.Users;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CRM.SocialDepartment.Application.Common
{
    /// <summary>
    /// Сервис для инициализации администратора при первом запуске приложения
    /// </summary>
    public class AdminInitializationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleAppService _roleAppService;
        private readonly UserAppService _userAppService;
        private readonly ILogger<AdminInitializationService> _logger;

        public AdminInitializationService(
            IUnitOfWork unitOfWork,
            RoleAppService roleAppService,
            UserAppService userAppService,
            ILogger<AdminInitializationService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleAppService = roleAppService;
            _userAppService = userAppService;
            _logger = logger;
        }

        /// <summary>
        /// Инициализирует администратора, если его не существует
        /// </summary>
        public async Task InitializeAdminAsync()
        {
            try
            {
                _logger.LogInformation("Начинаю проверку наличия администратора в системе");

                // Проверяем, есть ли роль Admin
                var adminRole = await _roleAppService.GetRoleByNameAsync("Admin");
                if (adminRole == null)
                {
                    _logger.LogInformation("Роль Admin не найдена, создаю...");
                    await CreateAdminRoleAsync();
                }

                // Проверяем, есть ли пользователи с ролью Admin
                var adminUsers = await GetUsersWithAdminRoleAsync();
                if (!adminUsers.Any())
                {
                    _logger.LogInformation("Пользователи с ролью Admin не найдены, создаю администратора...");
                    await CreateAdminUserAsync();
                }
                else
                {
                    _logger.LogInformation($"Найдено {adminUsers.Count()} пользователей с ролью Admin");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при инициализации администратора");
                throw;
            }
        }

        /// <summary>
        /// Создает роль Admin
        /// </summary>
        private async Task CreateAdminRoleAsync()
        {
            var createRoleDto = new CreateRoleDTO { Name = "Admin" };
            var result = await _roleAppService.CreateRoleAsync(createRoleDto);
            
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"Не удалось создать роль Admin: {result.Errors}");
            }
            
            _logger.LogInformation("Роль Admin успешно создана");
        }

        /// <summary>
        /// Создает пользователя Admin с ролью Admin
        /// </summary>
        private async Task CreateAdminUserAsync()
        {
            var createUserDto = new CreateUserDTO
            {
                UserName = "Admin",
                FirstName = "Администратор",
                LastName = "Системы",
                Email = "admin@system.local",
                Password = "Admin",
                Role = "Admin",
                Position = "Системный администратор",
                DepartmentNumber = "000"
            };

            var result = await _userAppService.CreateUserAsync(createUserDto);
            
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"Не удалось создать пользователя Admin: {result.Errors}");
            }
            
            _logger.LogInformation("Пользователь Admin с ролью Admin успешно создан");
        }

        private async Task<IEnumerable<object>> GetUsersWithAdminRoleAsync()
        {
            try
            {
                // Получаем роль Admin
                var adminRole = await _roleAppService.GetRoleByNameAsync("Admin");
                if (adminRole == null)
                {
                    return Enumerable.Empty<object>();
                }

                // Получаем всех пользователей с ролью Admin через UserRoles
                var adminUserRoles = await _unitOfWork.UserRoles.GetRoleUsersAsync(adminRole.Id);
                
                if (!adminUserRoles.Any())
                {
                    return Enumerable.Empty<object>();
                }

                // Получаем всех пользователей и фильтруем по ID
                var allUsers = _unitOfWork.Users.GetAllUsers();
                var adminUsers = new List<object>();
                
                foreach (var userRole in adminUserRoles)
                {
                    var user = allUsers.FirstOrDefault(u => 
                    {
                        if (u is IUser iUser)
                        {
                            return iUser.GetId() == userRole.UserId;
                        }
                        return false;
                    });
                    
                    if (user != null)
                    {
                        adminUsers.Add(user);
                    }
                }

                return adminUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей с ролью Admin");
                return Enumerable.Empty<object>();
            }
        }
    }
}
