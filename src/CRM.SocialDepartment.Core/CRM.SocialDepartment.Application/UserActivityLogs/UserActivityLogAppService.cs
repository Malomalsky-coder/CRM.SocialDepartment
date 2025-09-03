using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Repositories;
using System.Globalization;

namespace CRM.SocialDepartment.Application.UserActivityLogs
{
    public class UserActivityLogAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserActivityLogAppService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> AddLogAsync(UserActivityLog log)
        {
            return await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetAllLogsAsync()
        {
            var logs = await _unitOfWork.UserActivityLogs.GetAllAsync();
            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetFilteredLogsAsync(UserActivityLogFilterDTO filter)
        {
            var logs = await _unitOfWork.UserActivityLogs.GetFilteredAsync(
                filter.UserId,
                filter.ActivityType,
                filter.StartDate,
                filter.EndDate,
                filter.EntityType,
                filter.Page,
                filter.PageSize);

            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetUserLogsAsync(Guid userId)
        {
            var logs = await _unitOfWork.UserActivityLogs.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetLogsByActivityTypeAsync(UserActivityType activityType)
        {
            var logs = await _unitOfWork.UserActivityLogs.GetByActivityTypeAsync(activityType);
            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var logs = await _unitOfWork.UserActivityLogs.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<IEnumerable<UserActivityLogDTO>> GetLogsByEntityAsync(string entityType, Guid entityId)
        {
            var logs = await _unitOfWork.UserActivityLogs.GetByEntityAsync(entityType, entityId);
            return _mapper.Map<IEnumerable<UserActivityLogDTO>>(logs);
        }

        public async Task<UserActivityStatisticsDTO> GetActivityStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var allLogs = await _unitOfWork.UserActivityLogs.GetFilteredAsync(
                startDate: startDate,
                endDate: endDate);

            var statistics = new UserActivityStatisticsDTO
            {
                TotalLogs = allLogs.Count(),
                LoginCount = allLogs.Count(l => l.ActivityType == UserActivityType.Login),
                DataRequestCount = allLogs.Count(l => l.ActivityType == UserActivityType.DataRequest),
                CreateCount = allLogs.Count(l => l.ActivityType == UserActivityType.Create),
                UpdateCount = allLogs.Count(l => l.ActivityType == UserActivityType.Update),
                DeleteCount = allLogs.Count(l => l.ActivityType == UserActivityType.Delete)
            };

            // Статистика по пользователям
            statistics.ActivityByUser = allLogs
                .GroupBy(l => l.FullName)
                .ToDictionary(g => g.Key, g => g.Count());

            // Статистика по типам сущностей
            statistics.ActivityByEntityType = allLogs
                .Where(l => !string.IsNullOrEmpty(l.EntityType))
                .GroupBy(l => l.EntityType!)
                .ToDictionary(g => g.Key, g => g.Count());

            // Статистика по датам
            statistics.ActivityByDate = allLogs
                .GroupBy(l => l.Timestamp.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return statistics;
        }

        public async Task<int> GetLogsCountAsync(UserActivityLogFilterDTO filter)
        {
            return await _unitOfWork.UserActivityLogs.GetCountAsync(
                filter.UserId,
                filter.ActivityType,
                filter.StartDate,
                filter.EndDate,
                filter.EntityType);
        }

        public async Task<Result> DeleteOldLogsAsync(DateTime olderThan)
        {
            return await _unitOfWork.UserActivityLogs.DeleteOldLogsAsync(olderThan);
        }
    }
}
