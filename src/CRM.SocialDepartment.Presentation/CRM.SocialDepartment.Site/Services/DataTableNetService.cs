namespace CRM.SocialDepartment.Site.Services
{
    public class DataTableNetService
    {
        public DataTableNetModel Parse(HttpRequest httpRequest)
        {
            if (!httpRequest.HasFormContentType)
            {
                return new DataTableNetModel();
            }

            var form = httpRequest.Form;
            var draw = form["draw"].FirstOrDefault();
            var sortColumn = form["columns[" + form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = form["order[0][dir]"].FirstOrDefault();
            var searchValue = form["search[value]"].FirstOrDefault();
            _ = int.TryParse(form["length"].FirstOrDefault() ?? "0", out int pageSize);
            _ = int.TryParse(form["start"].FirstOrDefault() ?? "0", out int skip);

            return new DataTableNetModel
            {
                Draw = draw,
                SortColumn = sortColumn,
                SortColumnDirection = sortColumnDirection,
                SearchTerm = searchValue,
                PageSize = pageSize,
                Skip = skip
            };
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
