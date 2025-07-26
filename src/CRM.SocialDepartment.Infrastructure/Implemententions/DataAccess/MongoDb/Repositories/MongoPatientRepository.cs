using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Repositories;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория пациентов
    /// </summary>
    public class MongoPatientRepository : MongoBasicRepository<Patient, Guid>, IPatientRepository
    {
        public MongoPatientRepository(IMongoDatabase database) : base(database, "Patients")
        {
        }

        public MongoPatientRepository(IMongoDatabase database, IClientSessionHandle? session) : base(database, "Patients", session)
        {
        }

        /// <summary>
        /// Получить всех активных пациентов (находящихся в больнице)
        /// </summary>
        public async Task<IEnumerable<Patient>> GetActivePatientsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(p => !p.IsArchive && !p.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить пациентов по номеру отделения
        /// </summary>
        public async Task<IEnumerable<Patient>> GetPatientsByDepartmentAsync(sbyte departmentNumber, CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(p => p.ActiveHistory != null && 
                                         p.ActiveHistory.NumberDepartment == departmentNumber && 
                                         !p.IsArchive && 
                                         !p.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить архивных пациентов (выписанных)
        /// </summary>
        public async Task<IEnumerable<Patient>> GetArchivedPatientsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(p => p.IsArchive && !p.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Поиск пациентов по ФИО (частичное совпадение)
        /// </summary>
        public async Task<IEnumerable<Patient>> SearchByFullNameAsync(string fullName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return Enumerable.Empty<Patient>();

            var searchPattern = fullName.Trim().ToLowerInvariant();
            return await GetAllAsync(p => p.FullName.ToLower().Contains(searchPattern) && !p.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Проверить существует ли пациент с указанным номером документа
        /// </summary>
        public async Task<bool> ExistsWithDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            var patient = await GetAsync(p => p.Documents.Values.Any(doc => doc.Number == documentNumber) && !p.SoftDeleted, cancellationToken);
            return patient != null;
        }

        /// <summary>
        /// Получить активных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetActivePatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            // Базовый фильтр для активных пациентов
            var baseFilter = Builders<Patient>.Filter.And(
                Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false),
                Builders<Patient>.Filter.Eq(p => p.IsArchive, false)
            );

            // Фильтр поиска
            var searchFilter = baseFilter;
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                var nameFilter = Builders<Patient>.Filter.Regex(p => p.FullName, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                searchFilter = Builders<Patient>.Filter.And(baseFilter, nameFilter);
            }

            // Получить общее количество записей
            var totalRecords = _session != null 
                ? await collection.CountDocumentsAsync(_session, baseFilter, cancellationToken: cancellationToken)
                : await collection.CountDocumentsAsync(baseFilter, cancellationToken: cancellationToken);

            // Получить количество отфильтрованных записей
            var filteredRecords = _session != null
                ? await collection.CountDocumentsAsync(_session, searchFilter, cancellationToken: cancellationToken)
                : await collection.CountDocumentsAsync(searchFilter, cancellationToken: cancellationToken);

            // Получить данные с пагинацией
            var patients = _session != null
                ? await collection.Find(_session, searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken)
                : await collection.Find(searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken);

            return new DataTableResult<Patient>
            {
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords,
                Data = patients
            };
        }

        /// <summary>
        /// Получить архивных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetArchivedPatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            // Базовый фильтр для архивных пациентов
            var baseFilter = Builders<Patient>.Filter.And(
                Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false),
                Builders<Patient>.Filter.Eq(p => p.IsArchive, true)
            );

            // Фильтр поиска
            var searchFilter = baseFilter;
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                var nameFilter = Builders<Patient>.Filter.Regex(p => p.FullName, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                searchFilter = Builders<Patient>.Filter.And(baseFilter, nameFilter);
            }

            // Получить общее количество записей
            var totalRecords = _session != null 
                ? await collection.CountDocumentsAsync(_session, baseFilter, cancellationToken: cancellationToken)
                : await collection.CountDocumentsAsync(baseFilter, cancellationToken: cancellationToken);

            // Получить количество отфильтрованных записей
            var filteredRecords = _session != null
                ? await collection.CountDocumentsAsync(_session, searchFilter, cancellationToken: cancellationToken)
                : await collection.CountDocumentsAsync(searchFilter, cancellationToken: cancellationToken);

            // Получить данные с пагинацией
            var patients = _session != null
                ? await collection.Find(_session, searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken)
                : await collection.Find(searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken);

            return new DataTableResult<Patient>
            {
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords,
                Data = patients
            };
        }
    }
} 