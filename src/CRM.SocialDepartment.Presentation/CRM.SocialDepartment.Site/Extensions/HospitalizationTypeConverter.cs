using CRM.SocialDepartment.Domain.Entities.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRM.SocialDepartment.Site.Extensions
{
    /// <summary>
    /// Конвертер для преобразования строки в HospitalizationType
    /// </summary>
    public class HospitalizationTypeConverter : IModelBinder
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
                // Пытаемся конвертировать строку в byte, а затем в HospitalizationType
                if (byte.TryParse(value, out byte byteValue))
                {
                    var hospitalizationType = HospitalizationType.FromValue(byteValue);
                    bindingContext.Result = ModelBindingResult.Success(hospitalizationType);
                }
                else
                {
                    // Если не удалось распарсить как число, пробуем как отображаемое имя
                    var hospitalizationType = HospitalizationType.FromDisplayName(value);
                    bindingContext.Result = ModelBindingResult.Success(hospitalizationType);
                }
            }
            catch (ArgumentException ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Провайдер для конвертера HospitalizationType
    /// </summary>
    public class HospitalizationTypeConverterProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(HospitalizationType))
            {
                return new HospitalizationTypeConverter();
            }

            return null;
        }
    }
} 