using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace CRM.SocialDepartment.Site.Extensions
{
    /// <summary>
    /// Конвертер для преобразования строки в DocumentType
    /// </summary>
    public class DocumentTypeConverter : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            try
            {
                // Пытаемся конвертировать строку в DocumentType
                DocumentType documentType = value switch
                {
                    "0" or "Passport" or "Паспорт" => DocumentType.Passport,
                    "1" or "MedicalPolicy" or "Медицинский полис" => DocumentType.MedicalPolicy,
                    "2" or "Snils" or "СНИЛС" => DocumentType.Snils,
                    _ => throw new ArgumentException($"Неизвестный тип документа: {value}")
                };

                bindingContext.Result = ModelBindingResult.Success(documentType);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, 
                    $"Не удалось конвертировать '{value}' в тип документа: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Провайдер для DocumentType конвертера
    /// </summary>
    public class DocumentTypeConverterProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Поддерживаем DocumentType и Dictionary с DocumentType
            if (context.Metadata.ModelType == typeof(DocumentType))
            {
                return new DocumentTypeConverter();
            }

            // Для Dictionary<DocumentType, T> используем специальный конвертер
            if (context.Metadata.ModelType.IsGenericType && 
                context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                context.Metadata.ModelType.GetGenericArguments()[0] == typeof(DocumentType))
            {
                return new DocumentTypeDictionaryConverter();
            }

            return null;
        }
    }

    /// <summary>
    /// Конвертер для Dictionary<DocumentType, T>
    /// </summary>
    public class DocumentTypeDictionaryConverter : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var dictionary = new Dictionary<DocumentType, object>();
            var prefix = bindingContext.ModelName;

            // Простая реализация - возвращаем пустой словарь
            // Сложная логика обработки Dictionary будет реализована в AutoMapper
            bindingContext.Result = ModelBindingResult.Success(dictionary);
            return Task.CompletedTask;
        }
    }
} 