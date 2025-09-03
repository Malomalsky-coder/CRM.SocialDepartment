using CRM.SocialDepartment.Application.DTOs;

namespace CRM.SocialDepartment.Site.ViewModels.UserActivityLog
{
    /// <summary>
    /// ViewModel для отображения списка логов активности
    /// </summary>
    public class UserActivityLogListViewModel
    {
        public List<UserActivityLogDTO> Logs { get; set; } = new();
        public UserActivityLogFilterViewModel Filter { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage => Filter.Page;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < PageCount;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
    }
}
