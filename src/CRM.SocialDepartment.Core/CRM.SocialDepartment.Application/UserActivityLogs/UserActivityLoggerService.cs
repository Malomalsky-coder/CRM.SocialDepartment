using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CRM.SocialDepartment.Application.UserActivityLogs
{
    public class UserActivityLoggerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserActivityLoggerService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogLoginAsync(Guid userId, string userName, string fullName)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

            var log = UserActivityLog.CreateLoginLog(userId, userName, fullName, ipAddress, userAgent);
            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        public async Task LogDataRequestAsync(Guid userId, string userName, string fullName, string actionName, string description)
        {
            var ipAddress = GetClientIpAddress();
            var requestUrl = GetRequestUrl();
            var httpMethod = GetHttpMethod();

            var log = UserActivityLog.CreateDataRequestLog(userId, userName, fullName, actionName, description, requestUrl, httpMethod, ipAddress);
            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }


        public async Task LogEntityCreationAsync(Guid userId, string userName, string fullName, string entityType, Guid entityId, string actionName, string description, object? entityData = null)
        {
            var ipAddress = GetClientIpAddress();

            var log = UserActivityLog.CreateEntityLog(userId, userName, fullName, UserActivityType.Create, entityType, entityId, actionName, description, entityData, ipAddress);
            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        public async Task LogEntityUpdateAsync(Guid userId, string userName, string fullName, string entityType, Guid entityId, string actionName, string description, object? beforeData = null, object? afterData = null)
        {
            var ipAddress = GetClientIpAddress();

            var log = UserActivityLog.CreateUpdateLog(userId, userName, fullName, entityType, entityId, actionName, description, beforeData, afterData, ipAddress);
            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        public async Task LogEntityDeletionAsync(Guid userId, string userName, string fullName, string entityType, Guid entityId, string actionName, string description, object? entityData = null)
        {
            var ipAddress = GetClientIpAddress();

            var log = UserActivityLog.CreateEntityLog(userId, userName, fullName, UserActivityType.Delete, entityType, entityId, actionName, description, entityData, ipAddress);
            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        public (Guid userId, string userName, string fullName)? GetCurrentUserInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return null;

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
            var firstNameClaim = httpContext.User.FindFirst("FirstName");
            var lastNameClaim = httpContext.User.FindFirst("LastName");

            if (userIdClaim == null || userNameClaim == null)
                return null;

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            var userName = userNameClaim.Value;
            var firstName = firstNameClaim?.Value ?? "";
            var lastName = lastNameClaim?.Value ?? "";
            var fullName = $"{firstName} {lastName}".Trim();

            return (userId, userName, fullName);
        }

        public async Task LogActionAsync(UserActivityType activityType, string actionName, string description, string? entityType = null, Guid? entityId = null, object? beforeData = null, object? afterData = null)
        {
            var userInfo = GetCurrentUserInfo();
            if (userInfo == null)
                return;

            var (userId, userName, fullName) = userInfo.Value;
            var ipAddress = GetClientIpAddress();

            UserActivityLog log;

            switch (activityType)
            {
                case UserActivityType.DataRequest:
                    var requestUrl = GetRequestUrl();
                    var httpMethod = GetHttpMethod();
                    log = UserActivityLog.CreateDataRequestLog(userId, userName, fullName, actionName, description, requestUrl, httpMethod, ipAddress);
                    break;

                case UserActivityType.Create:
                    log = UserActivityLog.CreateEntityLog(userId, userName, fullName, activityType, entityType!, entityId!.Value, actionName, description, afterData, ipAddress);
                    break;

                case UserActivityType.Update:
                    log = UserActivityLog.CreateUpdateLog(userId, userName, fullName, entityType!, entityId!.Value, actionName, description, beforeData, afterData, ipAddress);
                    break;

                case UserActivityType.Delete:
                    log = UserActivityLog.CreateEntityLog(userId, userName, fullName, activityType, entityType!, entityId!.Value, actionName, description, beforeData, ipAddress);
                    break;

                default:
                    return;
            }

            await _unitOfWork.UserActivityLogs.AddAsync(log);
        }

        private string? GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Проверяем заголовки для получения реального IP адреса
            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            var realIpHeader = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIpHeader))
            {
                return realIpHeader;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault();
        }

        private string? GetRequestUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
        }

        private string? GetHttpMethod()
        {
            return _httpContextAccessor.HttpContext?.Request.Method;
        }
    }
}
