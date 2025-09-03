using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Linq;
using CRM.SocialDepartment.Domain.Repositories;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomUserClaimsPrincipalFactory> _logger;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IUnitOfWork unitOfWork,
            ILogger<CustomUserClaimsPrincipalFactory> logger)
            : base(userManager, roleManager, optionsAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = principal.Identity as ClaimsIdentity;

            if (identity != null)
            {
                try
                {
                    _logger.LogInformation("CustomUserClaimsPrincipalFactory: Создаем Claims для пользователя {UserName}", user.UserName);

                    // Получаем роли пользователя из кастомной таблицы userRoles
                    var userRoles = await _unitOfWork.UserRoles.GetUserRolesAsync(user.Id);
                    _logger.LogInformation("CustomUserClaimsPrincipalFactory: Найдено ролей пользователя: {Count}", userRoles.Count());

                    // Удаляем стандартные Claims ролей Identity
                    var existingRoleClaims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                    foreach (var claim in existingRoleClaims)
                    {
                        identity.RemoveClaim(claim);
                    }

                    // Добавляем роли из кастомной системы
                    foreach (var userRole in userRoles)
                    {
                        var role = await _unitOfWork.Roles.GetAsync(r => r.Id == userRole.RoleId);
                        if (role != null)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                            _logger.LogInformation("CustomUserClaimsPrincipalFactory: Добавлена роль {RoleName} для пользователя {UserName}", 
                                role.Name, user.UserName);
                        }
                        else
                        {
                            _logger.LogWarning("CustomUserClaimsPrincipalFactory: Роль с ID {RoleId} не найдена", userRole.RoleId);
                        }
                    }

                    _logger.LogInformation("CustomUserClaimsPrincipalFactory: Итоговые роли пользователя {UserName}: {Roles}", 
                        user.UserName, string.Join(", ", identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CustomUserClaimsPrincipalFactory: Ошибка при создании Claims для пользователя {UserName}", user.UserName);
                }
            }

            return principal;
        }
    }
}
