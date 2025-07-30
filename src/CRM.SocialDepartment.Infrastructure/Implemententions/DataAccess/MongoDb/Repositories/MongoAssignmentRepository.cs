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
        public MongoAssignmentRepository(IMongoDatabase database) : base(database, "assignments")
        {
        }

        public MongoAssignmentRepository(IMongoDatabase database, IClientSessionHandle? session) : base(database, "assignments", session)
        {
        }

        /// <summary>
        /// Экранирует специальные символы регулярных выражений для безопасного поиска в MongoDB
        /// </summary>
        /// <param name="input">Входная строка поиска</param>
        /// <returns>Экранированная строка</returns>
        private static string EscapeRegexSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Список специальных символов регулярных выражений, которые нужно экранировать
            var specialChars = new[] { '\\', '^', '$', '.', '|', '?', '*', '+', '(', ')', '[', ']', '{', '}', '/' };
            
            var result = input;
            foreach (var specialChar in specialChars)
            {
                result = result.Replace(specialChar.ToString(), "\\" + specialChar);
            }
            
            return result;
        }

        /// <summary>
        /// Создает безопасный фильтр поиска с экранированием специальных символов
        /// </summary>
        /// <param name="searchTerm">Поисковый термин</param>
        /// <returns>Фильтр MongoDB</returns>
        private static FilterDefinition<Assignment> CreateSafeSearchFilter(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return Builders<Assignment>.Filter.Empty;

            try
            {
                // Экранируем специальные символы регулярных выражений
                var escapedSearchTerm = EscapeRegexSpecialCharacters(searchTerm.Trim());
                
                // Создаем регулярное выражения для поиска по описанию и исполнителю (case-insensitive)
                var regexPattern = new MongoDB.Bson.BsonRegularExpression(escapedSearchTerm, "i");
                
                var descriptionFilter = Builders<Assignment>.Filter.Regex(a => a.Description, regexPattern);
                var assigneeFilter = Builders<Assignment>.Filter.Regex(a => a.Assignee, regexPattern);
                
                return Builders<Assignment>.Filter.Or(descriptionFilter, assigneeFilter);
            }
            catch (Exception ex)
            {
                // Если не удалось создать регулярное выражение, используем простой текстовый поиск
                Console.WriteLine($"⚠️ [MongoAssignmentRepository] Ошибка создания regex для поиска: {ex.Message}");
                
                // Fallback на простое сравнение с Contains (менее эффективно, но безопасно)
                var lowerSearchTerm = searchTerm.ToLower();
                return Builders<Assignment>.Filter.Where(a => 
                    a.Description.ToLower().Contains(lowerSearchTerm) || 
                    a.Assignee.ToLower().Contains(lowerSearchTerm));
            }
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

            try
            {
                // Базовый фильтр для активных назначений
                var baseFilter = Builders<Assignment>.Filter.And(
                    Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                    Builders<Assignment>.Filter.Eq(a => a.IsArchive, false)
                );

                // Фильтр поиска с безопасным экранированием
                var searchFilter = baseFilter;
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    var nameFilter = CreateSafeSearchFilter(parameters.SearchTerm);
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
            catch (MongoDB.Driver.MongoCommandException ex) when (ex.Message.Contains("Regular expression is invalid"))
            {
                Console.WriteLine($"🚨 [MongoAssignmentRepository] Ошибка регулярного выражения в поиске активных назначений: {ex.Message}");
                Console.WriteLine($"📝 Поисковый термин: '{parameters.SearchTerm}'");
                
                // Возвращаем результат без фильтрации поиска при ошибке regex
                var fallbackFilter = Builders<Assignment>.Filter.And(
                    Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                    Builders<Assignment>.Filter.Eq(a => a.IsArchive, false)
                );

                var totalRecords = _session != null 
                    ? await collection.CountDocumentsAsync(_session, fallbackFilter, cancellationToken: cancellationToken)
                    : await collection.CountDocumentsAsync(fallbackFilter, cancellationToken: cancellationToken);

                var assignments = _session != null
                    ? await collection.Find(_session, fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken)
                    : await collection.Find(fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken);

                return new DataTableResult<Assignment>
                {
                    TotalRecords = totalRecords,
                    FilteredRecords = totalRecords,
                    Data = assignments
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 [MongoAssignmentRepository] Общая ошибка при получении активных назначений: {ex.Message}");
                throw; // Пробрасываем исключение дальше для обработки в контроллере
            }
        }

        /// <summary>
        /// Получить архивные назначения для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Assignment>> GetArchivedAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            try
            {
                // Базовый фильтр для архивных назначений
                var baseFilter = Builders<Assignment>.Filter.And(
                    Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                    Builders<Assignment>.Filter.Eq(a => a.IsArchive, true)
                );

                // Фильтр поиска с безопасным экранированием
                var searchFilter = baseFilter;
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    var nameFilter = CreateSafeSearchFilter(parameters.SearchTerm);
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
            catch (MongoDB.Driver.MongoCommandException ex) when (ex.Message.Contains("Regular expression is invalid"))
            {
                Console.WriteLine($"🚨 [MongoAssignmentRepository] Ошибка регулярного выражения в поиске архивных назначений: {ex.Message}");
                Console.WriteLine($"📝 Поисковый термин: '{parameters.SearchTerm}'");
                
                // Возвращаем результат без фильтрации поиска при ошибке regex
                var fallbackFilter = Builders<Assignment>.Filter.And(
                    Builders<Assignment>.Filter.Eq(a => a.SoftDeleted, false),
                    Builders<Assignment>.Filter.Eq(a => a.IsArchive, true)
                );

                var totalRecords = _session != null 
                    ? await collection.CountDocumentsAsync(_session, fallbackFilter, cancellationToken: cancellationToken)
                    : await collection.CountDocumentsAsync(fallbackFilter, cancellationToken: cancellationToken);

                var assignments = _session != null
                    ? await collection.Find(_session, fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken)
                    : await collection.Find(fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken);

                return new DataTableResult<Assignment>
                {
                    TotalRecords = totalRecords,
                    FilteredRecords = totalRecords,
                    Data = assignments
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 [MongoAssignmentRepository] Общая ошибка при получении архивных назначений: {ex.Message}");
                throw; // Пробрасываем исключение дальше для обработки в контроллере
            }
        }
    }
} 