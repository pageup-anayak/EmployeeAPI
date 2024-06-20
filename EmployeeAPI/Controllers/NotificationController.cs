using EmployeeAPI.Contracts.Dtos.Requests.Notifications;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        public NotificationController(INotificationService notificationService,
                                      ILogger<NotificationController> logger,
                                      IUserService userService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _userService = userService;
        }

        #region Get Unseen Notitifcations
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Notification>))]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                int employeeId = int.Parse(_userService.GetUserIdFromToken());
                EmployeeType employeeRole = _userService.GetUserRoleFromToken();
                _logger.LogInformation($"Fetching notifications for Employee ID: {employeeId}, Role: {employeeRole}");
                List<Notification> notifications = (List<Notification>)await _notificationService.GetEmployeeNotificationsAsync(employeeId, employeeRole);

                _logger.LogInformation($"Fetched {notifications.Count} notifications for Employee ID: {employeeId}");
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching notifications: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching notifications");
            }
        }
        #endregion

        #region Mark as Seen
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Notification>))]
        public async Task<IActionResult> MarkNotificationsAsSeen([FromBody] MarkNotificationsAsSeenRequest notificationIds)
        {
            try
            {
                _logger.LogInformation($"Marking notifications as seen for IDs: {string.Join(",", notificationIds.Ids)}");
                if (!await _notificationService.NotificationsExistsAsync(notificationIds))
                {
                    _logger.LogWarning($"Some notification IDs do not exist: {string.Join(",", notificationIds.Ids)}");
                    return BadRequest("Some notificaions ids dose not exists");
                }
                List<int> alreadySeenNotifications = (List<int>)await _notificationService.MarkNotificationsAsReadAsync(notificationIds);
                if (alreadySeenNotifications.Count > 0)
                {
                    _logger.LogInformation($"Already seen notifications: {string.Join(",", alreadySeenNotifications)}");
                    return BadRequest($"These are the already seen the notifications {alreadySeenNotifications}");
                }
                _logger.LogInformation($"Marked {notificationIds.Ids.Count} notifications as seen");
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking notifications as seen: {ex.Message}");
                return StatusCode(500, "An error occurred while marking notifications as seen");
            }
        }
        #endregion
    }
}
