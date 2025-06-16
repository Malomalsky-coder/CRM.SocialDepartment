namespace CRM.SocialDepartment.Site.Models
{
    /// <summary>
    /// Стандартизированный ответ API
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Успешность операции
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Данные ответа
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Тип сообщения для SweetAlert
        /// </summary>
        public string? MessageType { get; set; }

        /// <summary>
        /// Создает успешный ответ
        /// </summary>
        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                MessageType = "success"
            };
        }

        /// <summary>
        /// Создает ответ с ошибкой
        /// </summary>
        public static ApiResponse<T> Error(string message, T? data, string messageType = "error")
        {
            return new ApiResponse<T>
            {
                Success = false,
                ErrorMessage = message,
                MessageType = messageType,
                Data = data
            };
        }
    }
} 