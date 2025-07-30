using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CRM.SocialDepartment.Domain.Common
{
    public static class EnumExtensions
    {
        public static T? GetAttribute<T>(this int value) where T : Attribute
        {
            // Для ValueObject нужно найти соответствующий enum или создать маппинг
            // Пока возвращаем null, так как у нас нет прямого enum
            return null;
        }
        
        public static string GetDisplayName(this int value)
        {
            // Простое отображение значения как строки
            return value.ToString();
        }
    }
} 