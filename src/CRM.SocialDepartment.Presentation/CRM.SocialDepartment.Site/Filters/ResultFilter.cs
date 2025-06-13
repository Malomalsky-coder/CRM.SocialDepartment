using CRM.SocialDepartment.Domain.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Filters
{
    public class ResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            // Если результат - это ObjectResult (возвращаемый контроллером)
            if (context.Result is ObjectResult objectResult)
            {
                // Обрабатываем Result<T>
                if (objectResult.Value is Result result)
                {
                    if (!result.IsSuccess)
                    {
                        context.Result = new BadRequestObjectResult(result.Errors);
                    }
                    else
                    {
                        // Для Result<T> возвращаем Value
                        if (objectResult.Value is IResultWithValue resultWithValue)
                        {
                            context.Result = new OkObjectResult(resultWithValue.Value);
                        }
                        else
                        {
                            context.Result = new OkResult(); // Пустой ответ 200 для необобщённого Result
                        }
                    }
                }
            }

            await next();
        }
    }
}
