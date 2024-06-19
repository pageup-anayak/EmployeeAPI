using EmployeeAPI.Contracts.Interfaces;
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
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        #region Get Unseen Notitifcations
        #endregion


        #region Mark as Seen
        #endregion
    }
}
