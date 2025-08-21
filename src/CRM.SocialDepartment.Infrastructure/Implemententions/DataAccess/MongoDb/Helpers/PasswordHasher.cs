using Microsoft.AspNetCore.Identity;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Helpers
{
    /// <summary>
    /// Вспомогательный класс для хеширования паролей
    /// Использует тот же алгоритм, что и SignInManager
    /// </summary>
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<ApplicationUser> _hasher = new();

        /// <summary>
        /// Хеширует пароль используя тот же алгоритм, что и SignInManager
        /// </summary>
        /// <param name="password">Пароль для хеширования</param>
        /// <returns>Хешированный пароль</returns>
        public static string HashPassword(ApplicationUser user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        /// <summary>
        /// Проверяет пароль используя тот же алгоритм, что и SignInManager
        /// </summary>
        /// <param name="hashedPassword">Хешированный пароль</param>
        /// <param name="providedPassword">Предоставленный пароль</param>
        /// <returns>Результат проверки</returns>
        public static PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
        {
            return _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        }
    }
} 