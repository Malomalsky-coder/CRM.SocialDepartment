namespace CRM.SocialDepartment.Site.Services
{
    public class DataTableNetService
    {
        public DataTableNetModel Parse(HttpRequest httpRequest)
        {
            Console.WriteLine($"🔍 [DataTableNetService] Парсим запрос: {httpRequest.Method} {httpRequest.Path}");
            Console.WriteLine($"🔍 [DataTableNetService] Content-Type: {httpRequest.ContentType}");
            
            if (!httpRequest.HasFormContentType)
            {
                Console.WriteLine("⚠️ [DataTableNetService] Запрос не содержит form data");
                return new DataTableNetModel();
            }

            var form = httpRequest.Form;
            Console.WriteLine($"🔍 [DataTableNetService] Ключи формы: {string.Join(", ", form.Keys)}");
            
            // Проверяем наличие минимально необходимых параметров
            if (!form.ContainsKey("draw") || !form.ContainsKey("length") || !form.ContainsKey("start"))
            {
                Console.WriteLine("⚠️ [DataTableNetService] Отсутствуют обязательные параметры формы");
                return new DataTableNetModel
                {
                    Draw = "1",
                    PageSize = 10,
                    Skip = 0,
                    SearchTerm = "",
                    SortColumn = null,
                    SortColumnDirection = null
                };
            }
            
            var draw = form["draw"].FirstOrDefault();
            var searchValue = form["search[value]"].FirstOrDefault();
            _ = int.TryParse(form["length"].FirstOrDefault() ?? "0", out int pageSize);
            _ = int.TryParse(form["start"].FirstOrDefault() ?? "0", out int skip);

            Console.WriteLine($"🔍 [DataTableNetService] Draw: {draw}, SearchValue: '{searchValue}', PageSize: {pageSize}, Skip: {skip}");

            // Проверяем наличие обязательных параметров
            if (string.IsNullOrEmpty(draw))
            {
                Console.WriteLine("⚠️ [DataTableNetService] Отсутствует параметр 'draw'");
                draw = "1";
            }
            
            // Обрабатываем поисковый параметр
            if (string.IsNullOrEmpty(searchValue))
            {
                Console.WriteLine("🔍 [DataTableNetService] Поисковый параметр пустой");
                searchValue = "";
            }
            
            // Проверяем корректность числовых параметров
            if (pageSize <= 0) pageSize = 10;
            if (skip < 0) skip = 0;

            // Обработка сортировки
            string? sortColumn = null;
            string? sortDirection = null;

            if (form.ContainsKey("order[0][column]") && form.ContainsKey("order[0][dir]"))
            {
                var orderColumnIndex = form["order[0][column]"].FirstOrDefault();
                var orderDirection = form["order[0][dir]"].FirstOrDefault();

                if (!string.IsNullOrEmpty(orderColumnIndex) && !string.IsNullOrEmpty(orderDirection))
                {
                    // Получаем имя колонки по индексу
                    var columnIndex = int.Parse(orderColumnIndex);
                    var columnName = form[$"columns[{columnIndex}][name]"].FirstOrDefault();
                    
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        sortColumn = columnName;
                        sortDirection = orderDirection;
                    }
                }
            }

            var result = new DataTableNetModel
            {
                Draw = draw,
                SortColumn = sortColumn,
                SortColumnDirection = sortDirection,
                SearchTerm = searchValue,
                PageSize = pageSize,
                Skip = skip
            };
            
            Console.WriteLine($"📋 [DataTableNetService] Результат парсинга: {@result}");
            return result;
        }
    }

    public class DataTableNetModel
    {
        public string? Draw { get; set; }

        public string? SortColumn { get; set; }

        public string? SortColumnDirection { get; set; }

        public string? SearchTerm { get; set; }

        public int PageSize { get; set; }

        public int Skip { get; set; }
    }
}
