using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.UserActivityLogs;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Site.ViewModels.UserActivityLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers
{
    /// <summary>
    /// Контроллер для просмотра логов активности пользователей
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UserActivityLogController : Controller
    {
        private readonly UserActivityLogAppService _userActivityLogAppService;
        private readonly ILogger<UserActivityLogController> _logger;

        public UserActivityLogController(
            UserActivityLogAppService userActivityLogAppService,
            ILogger<UserActivityLogController> logger)
        {
            _userActivityLogAppService = userActivityLogAppService;
            _logger = logger;
        }

        /// <summary>
        /// Главная страница с логами активности
        /// </summary>
        public async Task<IActionResult> Index(UserActivityLogFilterViewModel filter)
        {
            try
            {
                var filterDto = new UserActivityLogFilterDTO
                {
                    UserId = filter.UserId,
                    ActivityType = filter.ActivityType,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    EntityType = filter.EntityType,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                };

                var logs = await _userActivityLogAppService.GetFilteredLogsAsync(filterDto);
                var totalCount = await _userActivityLogAppService.GetLogsCountAsync(filterDto);

                var viewModel = new UserActivityLogListViewModel
                {
                    Logs = logs.ToList(),
                    Filter = filter,
                    TotalCount = totalCount,
                    PageCount = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении логов активности");
                TempData["Error"] = "Произошла ошибка при загрузке логов активности";
                return View(new UserActivityLogListViewModel());
            }
        }

        /// <summary>
        /// Детальный просмотр лога
        /// </summary>
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var logs = await _userActivityLogAppService.GetFilteredLogsAsync(new UserActivityLogFilterDTO());
                var log = logs.FirstOrDefault(l => l.Id == id);

                if (log == null)
                {
                    return NotFound();
                }

                return View(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении деталей лога активности");
                TempData["Error"] = "Произошла ошибка при загрузке деталей лога";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Статистика активности
        /// </summary>
        public async Task<IActionResult> Statistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var statistics = await _userActivityLogAppService.GetActivityStatisticsAsync(startDate, endDate);
                return View(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики активности");
                TempData["Error"] = "Произошла ошибка при загрузке статистики";
                return View(new UserActivityStatisticsDTO());
            }
        }

        /// <summary>
        /// Логи активности пользователя
        /// </summary>
        public async Task<IActionResult> UserLogs(Guid userId)
        {
            try
            {
                var logs = await _userActivityLogAppService.GetUserLogsAsync(userId);
                return View(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении логов пользователя {UserId}", userId);
                TempData["Error"] = "Произошла ошибка при загрузке логов пользователя";
                return View(new List<UserActivityLogDTO>());
            }
        }

        /// <summary>
        /// Логи по типу действия
        /// </summary>
        public async Task<IActionResult> ActivityTypeLogs(UserActivityType activityType)
        {
            try
            {
                var logs = await _userActivityLogAppService.GetLogsByActivityTypeAsync(activityType);
                ViewBag.ActivityType = activityType;
                return View(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении логов по типу активности {ActivityType}", activityType);
                TempData["Error"] = "Произошла ошибка при загрузке логов";
                return View(new List<UserActivityLogDTO>());
            }
        }

        /// <summary>
        /// Логи по диапазону дат
        /// </summary>
        public async Task<IActionResult> DateRangeLogs(DateTime startDate, DateTime endDate)
        {
            try
            {
                var logs = await _userActivityLogAppService.GetLogsByDateRangeAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении логов по диапазону дат {StartDate} - {EndDate}", startDate, endDate);
                TempData["Error"] = "Произошла ошибка при загрузке логов";
                return View(new List<UserActivityLogDTO>());
            }
        }

        /// <summary>
        /// Логи по сущности
        /// </summary>
        public async Task<IActionResult> EntityLogs(string entityType, Guid entityId)
        {
            try
            {
                var logs = await _userActivityLogAppService.GetLogsByEntityAsync(entityType, entityId);
                ViewBag.EntityType = entityType;
                ViewBag.EntityId = entityId;
                return View(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении логов по сущности {EntityType} {EntityId}", entityType, entityId);
                TempData["Error"] = "Произошла ошибка при загрузке логов сущности";
                return View(new List<UserActivityLogDTO>());
            }
        }

        /// <summary>
        /// Экспорт логов в JSON
        /// </summary>
        public async Task<IActionResult> ExportJson(UserActivityLogFilterViewModel filter)
        {
            try
            {
                var filterDto = new UserActivityLogFilterDTO
                {
                    UserId = filter.UserId,
                    ActivityType = filter.ActivityType,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    EntityType = filter.EntityType
                };

                var logs = await _userActivityLogAppService.GetFilteredLogsAsync(filterDto);
                
                var fileName = $"user_activity_logs_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                return File(
                    System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(logs, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })),
                    "application/json",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте логов");
                TempData["Error"] = "Произошла ошибка при экспорте логов";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Очистка старых логов
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CleanupOldLogs(int daysToKeep = 90)
        {
            try
            {
                var olderThan = DateTime.UtcNow.AddDays(-daysToKeep);
                var result = await _userActivityLogAppService.DeleteOldLogsAsync(olderThan);

                if (result.IsSuccess)
                {
                    TempData["Success"] = $"Удалены логи старше {daysToKeep} дней";
                }
                else
                {
                    TempData["Error"] = $"Ошибка при удалении старых логов: {string.Join(", ", result.Errors)}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при очистке старых логов");
                TempData["Error"] = "Произошла ошибка при очистке старых логов";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
