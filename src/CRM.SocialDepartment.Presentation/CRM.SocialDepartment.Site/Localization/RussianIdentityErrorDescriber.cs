using Microsoft.AspNetCore.Identity;

namespace CRM.SocialDepartment.Site.Localization
{
    /// <summary>
    /// Кастомный IdentityErrorDescriber для русификации системных ошибок Identity
    /// </summary>
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = "Произошла неизвестная ошибка."
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = "Ошибка параллелизма, объект был изменен."
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = "Неверный пароль."
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = "Недействительный токен."
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = nameof(LoginAlreadyAssociated),
                Description = "Пользователь с данным логином уже существует."
            };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"Имя пользователя '{userName}' недействительно, может содержать только буквы и цифры."
            };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"Адрес электронной почты '{email}' недействителен."
            };
        }

        public override IdentityError DuplicateUserName(string? userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Имя пользователя '{userName}' уже занято."
            };
        }

        public override IdentityError DuplicateEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Адрес электронной почты '{email}' уже занят."
            };
        }

        public override IdentityError InvalidRoleName(string? role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = $"Название роли '{role}' недействительно."
            };
        }

        public override IdentityError DuplicateRoleName(string? role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = $"Роль '{role}' уже существует."
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = "У пользователя уже установлен пароль."
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = "Блокировка для данного пользователя не включена."
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = $"Пользователь уже находится в роли '{role}'."
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = $"Пользователь не находится в роли '{role}'."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"Пароль должен содержать минимум {length} символов."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Пароль должен содержать минимум один небуквенно-цифровой символ."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Пароль должен содержать минимум одну цифру ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Пароль должен содержать минимум одну строчную букву ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Пароль должен содержать минимум одну заглавную букву ('A'-'Z')."
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"Пароль должен содержать минимум {uniqueChars} уникальных символов."
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = "Не удалось использовать код восстановления."
            };
        }
    }
} 