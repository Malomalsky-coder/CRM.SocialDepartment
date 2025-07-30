using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Entities.Patients.Documents;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Specifications;
using MongoDB.Driver;
using DDD.Repositories;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core;

namespace CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB реализация репозитория пациентов
    /// </summary>
    public class MongoPatientRepository : MongoBasicRepository<Patient, Guid>, IPatientRepository
    {
        public MongoPatientRepository(IMongoDatabase database) : base(database, "patients")
        {
        }

        public MongoPatientRepository(IMongoDatabase database, IClientSessionHandle? session) : base(database, "patients", session)
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
        /// Создает безопасный фильтр поиска по FullName с экранированием специальных символов
        /// </summary>
        /// <param name="searchTerm">Поисковый термин</param>
        /// <returns>Фильтр MongoDB</returns>
        private static FilterDefinition<Patient> CreateSafeSearchFilter(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return Builders<Patient>.Filter.Empty;

            try
            {
                // Экранируем специальные символы регулярных выражений
                var escapedSearchTerm = EscapeRegexSpecialCharacters(searchTerm.Trim());
                
                // Создаем регулярное выражение для поиска (case-insensitive)
                var regexPattern = new MongoDB.Bson.BsonRegularExpression(escapedSearchTerm, "i");
                
                return Builders<Patient>.Filter.Regex(p => p.FullName, regexPattern);
            }
            catch (Exception ex)
            {
                // Если не удалось создать регулярное выражение, используем простой текстовый поиск
                Console.WriteLine($"⚠️ [MongoPatientRepository] Ошибка создания regex для поиска: {ex.Message}");
                
                // Fallback на простое сравнение с Contains (менее эффективно, но безопасно)
                return Builders<Patient>.Filter.Where(p => p.FullName.ToLower().Contains(searchTerm.ToLower()));
            }
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
            var collection = GetCollection();
            
            var baseFilter = Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false);
            var searchFilter = baseFilter;
            
            if (!string.IsNullOrEmpty(fullName))
            {
                var nameFilter = CreateSafeSearchFilter(fullName);
                searchFilter = Builders<Patient>.Filter.And(baseFilter, nameFilter);
            }

            return _session != null
                ? await collection.Find(_session, searchFilter).ToListAsync(cancellationToken)
                : await collection.Find(searchFilter).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Проверить существует ли пациент с указанным номером документа
        /// </summary>
        public async Task<bool> ExistsWithDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            // Получаем всех пациентов и проверяем документы в памяти
            var patients = await GetAllAsync(p => !p.SoftDeleted, cancellationToken);
            
            return patients.Any(p => p.Documents.Values.Any(doc => 
            {
                if (doc is PassportDocument passport) return passport.Number == documentNumber;
                if (doc is MedicalPolicyDocument policy) return policy.Number == documentNumber;
                if (doc is SnilsDocument snils) return snils.Number == documentNumber;
                return false;
            }));
        }

        /// <summary>
        /// Получить активных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetActivePatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            try
            {
                // Базовый фильтр для активных пациентов
                var baseFilter = Builders<Patient>.Filter.And(
                    Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false),
                    Builders<Patient>.Filter.Eq(p => p.IsArchive, false)
                );

                // Фильтр поиска с безопасным экранированием
                var searchFilter = baseFilter;
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    var nameFilter = CreateSafeSearchFilter(parameters.SearchTerm);
                    searchFilter = Builders<Patient>.Filter.And(baseFilter, nameFilter);
                }

                // Получить общее количество записей
                var totalRecords = _session != null 
                    ? await collection.CountDocumentsAsync(_session, baseFilter, cancellationToken: cancellationToken)
                    : await collection.CountDocumentsAsync(baseFilter, cancellationToken: cancellationToken);

                // Получить отфильтрованное количество записей
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

                // Применить сортировку если указана
                if (!string.IsNullOrEmpty(parameters.SortColumn) && !string.IsNullOrEmpty(parameters.SortDirection))
                {
                    var sortDirection = parameters.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? "descending" : "ascending";
                    
                    try
                    {
                        // Безопасная сортировка с обработкой null значений
                        patients = parameters.SortColumn switch
                        {
                            "HospitalizationType" => sortDirection == "ascending" 
                                ? [..patients.OrderBy(p => p.ActiveHistory?.HospitalizationType?.DisplayName ?? "")]
                                : [..patients.OrderByDescending(p => p.ActiveHistory?.HospitalizationType?.DisplayName ?? "")],
                            
                            "DateOfReceipt" => sortDirection == "ascending"
                                ? [..patients.OrderBy(p => p.ActiveHistory?.DateOfReceipt ?? DateTime.MinValue)]
                                : [..patients.OrderByDescending(p => p.ActiveHistory?.DateOfReceipt ?? DateTime.MinValue)],
                            
                            "Department" => sortDirection == "ascending"
                                ? [..patients.OrderBy(p => p.ActiveHistory?.NumberDepartment ?? 0)]
                                : [..patients.OrderByDescending(p => p.ActiveHistory?.NumberDepartment ?? 0)],
                            
                            "FullName" => sortDirection == "ascending"
                                ? [..patients.OrderBy(p => p.FullName ?? "")]
                                : [..patients.OrderByDescending(p => p.FullName ?? "")],
                            
                            "IsChildren" => sortDirection == "ascending"
                                ? patients.OrderBy(p => p.IsChildren).ToList()
                                : [..patients.OrderByDescending(p => p.IsChildren)],
                            
                            "IsCapable" => sortDirection == "ascending"
                                ? [..patients.OrderBy(p => p.IsCapable)]
                                : [..patients.OrderByDescending(p => p.IsCapable)],
                            
                            "ReceivesPension" => sortDirection == "ascending"
                                ? [..patients.OrderBy(p => p.ReceivesPension)]
                                : [..patients.OrderByDescending(p => p.ReceivesPension)],
                            
                            "DisabilityGroup" => patients, // Временно отключаем сортировку
                            
                            _ => patients // Для неизвестных полей оставляем исходный порядок
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ [MongoPatientRepository] Ошибка сортировки по полю {parameters.SortColumn}: {ex.Message}");
                        // Если сортировка не удалась, оставляем исходный порядок
                    }
                }

                return new DataTableResult<Patient>
                {
                    TotalRecords = totalRecords,
                    FilteredRecords = filteredRecords,
                    Data = patients
                };
            }
            catch (MongoDB.Driver.MongoCommandException ex) when (ex.Message.Contains("Regular expression is invalid"))
            {
                Console.WriteLine($"🚨 [MongoPatientRepository] Ошибка регулярного выражения в поиске: {ex.Message}");
                
                // Возвращаем пустой результат при ошибке регулярного выражения
                return new DataTableResult<Patient>
                {
                    TotalRecords = 0,
                    FilteredRecords = 0,
                    Data = Enumerable.Empty<Patient>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 [MongoPatientRepository] Общая ошибка при получении активных пациентов: {ex.Message}");
                throw; // Пробрасываем исключение дальше для обработки в контроллере
            }
        }

        /// <summary>
        /// Получить архивных пациентов для DataTables с поддержкой фильтрации и пагинации
        /// </summary>
        public async Task<DataTableResult<Patient>> GetArchivedPatientsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            try
            {
                // Базовый фильтр для архивных пациентов
                var baseFilter = Builders<Patient>.Filter.And(
                    Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false),
                    Builders<Patient>.Filter.Eq(p => p.IsArchive, true)
                );

                // Фильтр поиска с безопасным экранированием
                var searchFilter = baseFilter;
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    var nameFilter = CreateSafeSearchFilter(parameters.SearchTerm);
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
            catch (MongoDB.Driver.MongoCommandException ex) when (ex.Message.Contains("Regular expression is invalid"))
            {
                Console.WriteLine($"🚨 [MongoPatientRepository] Ошибка регулярного выражения в поиске архива: {ex.Message}");
                Console.WriteLine($"📝 Поисковый термин: '{parameters.SearchTerm}'");
                
                // Возвращаем результат без фильтрации поиска при ошибке regex
                var fallbackFilter = Builders<Patient>.Filter.And(
                    Builders<Patient>.Filter.Eq(p => p.SoftDeleted, false),
                    Builders<Patient>.Filter.Eq(p => p.IsArchive, true)
                );

                var totalRecords = _session != null 
                    ? await collection.CountDocumentsAsync(_session, fallbackFilter, cancellationToken: cancellationToken)
                    : await collection.CountDocumentsAsync(fallbackFilter, cancellationToken: cancellationToken);

                var patients = _session != null
                    ? await collection.Find(_session, fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken)
                    : await collection.Find(fallbackFilter)
                        .Skip(parameters.Skip)
                        .Limit(parameters.PageSize)
                        .ToListAsync(cancellationToken);

                return new DataTableResult<Patient>
                {
                    TotalRecords = totalRecords,
                    FilteredRecords = totalRecords,
                    Data = patients
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 [MongoPatientRepository] Общая ошибка при получении архивных пациентов: {ex.Message}");
                throw; // Пробрасываем исключение дальше для обработки в контроллере
            }
        }
    }
} 