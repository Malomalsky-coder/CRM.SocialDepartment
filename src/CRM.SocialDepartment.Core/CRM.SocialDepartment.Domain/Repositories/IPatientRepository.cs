using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
using DDD.Repositories;

namespace CRM.SocialDepartment.Domain.Repositories
{
    /// <summary>
    /// Доменный интерфейс репозитория для работы с пациентами
    /// </summary>
    public interface IPatientRepository : IBasicRepository<Patient, Guid>
    {
        /// <summary>
        /// Получить всех активных пациентов (находящихся в больнице)
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных пациентов</returns>
        Task<IEnumerable<Patient>> GetActivePatientsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить пациентов по номеру отделения
        /// </summary>
        /// <param name="departmentNumber">Номер отделения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список пациентов в отделении</returns>
        Task<IEnumerable<Patient>> GetPatientsByDepartmentAsync(sbyte departmentNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить архивных пациентов (выписанных)
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список архивных пациентов</returns>
        Task<IEnumerable<Patient>> GetArchivedPatientsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Поиск пациентов по ФИО (частичное совпадение)
        /// </summary>
        /// <param name="fullName">Полное или частичное ФИО</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список найденных пациентов</returns>
        Task<IEnumerable<Patient>> SearchByFullNameAsync(string fullName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить существует ли пациент с указанным номером документа
        /// </summary>
        /// <param name="documentNumber">Номер документа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если пациент существует</returns>
        Task<bool> ExistsWithDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить активных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        /// <param name="parameters">Параметры запроса DataTables</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат для DataTables</returns>
        Task<DataTableResult<Patient>> GetActivePatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить архивных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        /// <param name="parameters">Параметры запроса DataTables</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат для DataTables</returns>
        Task<DataTableResult<Patient>> GetArchivedPatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default);
    }
} 