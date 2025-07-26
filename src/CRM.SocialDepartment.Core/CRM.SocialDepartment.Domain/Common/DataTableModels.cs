namespace CRM.SocialDepartment.Domain.Common
{
    /// <summary>
    /// Результат запроса для DataTables
    /// </summary>
    public class DataTableResult<T>
    {
        public long TotalRecords { get; set; }
        public long FilteredRecords { get; set; }
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }

    /// <summary>
    /// Параметры для DataTables запроса
    /// </summary>
    public class DataTableParameters
    {
        public int Skip { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
    }
} 