using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Entities.Patients;
using DDD.Repositories;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Доменный интерфейс репозитория для работы с назначениями
    /// </summary>
    public interface IAssignmentRepository : IBasicRepository<Assignment, Guid>
    {
        /// <summary>
        /// Получить все активные назначения (не в архиве)
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных назначений</returns>
        Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить назначения по номеру отделения
        /// </summary>
        /// <param name="departmentNumber">Номер отделения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список назначений отделения</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsByDepartmentAsync(int departmentNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить назначения для конкретного пациента
        /// </summary>
        /// <param name="patientId">Идентификатор пациента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список назначений пациента</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить назначения по исполнителю
        /// </summary>
        /// <param name="assignee">Исполнитель</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список назначений исполнителя</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsByAssigneeAsync(string assignee, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить архивные назначения
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список архивных назначений</returns>
        Task<IEnumerable<Assignment>> GetArchivedAssignmentsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить назначения в определенном диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список назначений в диапазоне дат</returns>
        Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить активные назначения для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        /// <param name="parameters">Параметры запроса DataTables</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат для DataTables</returns>
        Task<DataTableResult<Assignment>> GetActiveAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить архивные назначения для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        /// <param name="parameters">Параметры запроса DataTables</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат для DataTables</returns>
        Task<DataTableResult<Assignment>> GetArchivedAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default);
    }
} 