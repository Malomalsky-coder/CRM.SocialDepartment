using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Repositories;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория назначений
    /// </summary>
    public class MongoAssignmentRepository : MongoBasicRepository<Assignment, Guid>, IAssignmentRepository
    {
        public MongoAssignmentRepository(IMongoDatabase database) : base(database, "Assignments")
        {
        }

        public MongoAssignmentRepository(IMongoDatabase database, IClientSessionHandle? session) : base(database, "Assignments", session)
        {
        }

        /// <summary>
        /// Получить все активные назначения (не в архиве)
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(a => !a.IsArchive && !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить назначения по номеру отделения
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetAssignmentsByDepartmentAsync(int departmentNumber, CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(a => a.DepartmentNumber == departmentNumber && !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить назначения для конкретного пациента
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetAssignmentsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(a => a.Patient.Id == patientId && !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить назначения по исполнителю
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetAssignmentsByAssigneeAsync(string assignee, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(assignee))
                return Enumerable.Empty<Assignment>();

            return await GetAllAsync(a => a.Assignee.Contains(assignee) && !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить архивные назначения
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetArchivedAssignmentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(a => a.IsArchive && !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить назначения в определенном диапазоне дат
        /// </summary>
        public async Task<IEnumerable<Assignment>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(a => a.CreationDate >= startDate && 
                                         a.CreationDate <= endDate && 
                                         !a.SoftDeleted, cancellationToken);
        }

        /// <summary>
        /// Получить активные назначения для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Assignment>> GetActiveAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            // Базовый фильтр для активных назначений
            var baseFilter = Builders<Assignment>.Filter.And(
                Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                Builders<Assignment>.Filter.Eq(a => a.IsArchive, false)
            );

            // Фильтр поиска
            var searchFilter = baseFilter;
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                var descriptionFilter = Builders<Assignment>.Filter.Regex(a => a.Description, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                var assigneeFilter = Builders<Assignment>.Filter.Regex(a => a.Assignee, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                var nameFilter = Builders<Assignment>.Filter.Or(descriptionFilter, assigneeFilter);
                searchFilter = Builders<Assignment>.Filter.And(baseFilter, nameFilter);
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
            var assignments = _session != null
                ? await collection.Find(_session, searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken)
                : await collection.Find(searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken);

            return new DataTableResult<Assignment>
            {
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords,
                Data = assignments
            };
        }

        /// <summary>
        /// Получить архивные назначения для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Assignment>> GetArchivedAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            // Базовый фильтр для архивных назначений
            var baseFilter = Builders<Assignment>.Filter.And(
                Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                Builders<Assignment>.Filter.Eq(a => a.IsArchive, true)
            );

            // Фильтр поиска
            var searchFilter = baseFilter;
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                var descriptionFilter = Builders<Assignment>.Filter.Regex(a => a.Description, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                var assigneeFilter = Builders<Assignment>.Filter.Regex(a => a.Assignee, 
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
                var nameFilter = Builders<Assignment>.Filter.Or(descriptionFilter, assigneeFilter);
                searchFilter = Builders<Assignment>.Filter.And(baseFilter, nameFilter);
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
            var assignments = _session != null
                ? await collection.Find(_session, searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken)
                : await collection.Find(searchFilter)
                    .Skip(parameters.Skip)
                    .Limit(parameters.PageSize)
                    .ToListAsync(cancellationToken);

            return new DataTableResult<Assignment>
            {
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords,
                Data = assignments
            };
        }
    }
} 