using CRM.SocialDepartment.Domain.Entities;

namespace CRM.SocialDepartment.Application.DTOs
{

    public class UserActivityLogDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserActivityType ActivityType { get; set; }
        public string ActivityTypeName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? BeforeChange { get; set; }
        public string? AfterChange { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? RequestUrl { get; set; }
        public string? HttpMethod { get; set; }
        public DateTime Timestamp { get; set; }
        public string FormattedTimestamp { get; set; } = string.Empty;
        public string? AdditionalData { get; set; }
    }

    public class UserActivityLogFilterDTO
    {
        public Guid? UserId { get; set; }
        public UserActivityType? ActivityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? EntityType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class UserActivityStatisticsDTO
    {
        public int TotalLogs { get; set; }
        public int LoginCount { get; set; }
        public int DataRequestCount { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int DeleteCount { get; set; }
        public Dictionary<string, int> ActivityByUser { get; set; } = new();
        public Dictionary<string, int> ActivityByEntityType { get; set; } = new();
        public Dictionary<DateTime, int> ActivityByDate { get; set; } = new();
    }
}
