using AutoMapper;
using EmployeeAPI.Contracts.Dtos.Requests.Notifications;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeAPI.Provider.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmployeeDBContext _context;
        private readonly ILogger<NotificationService> _logger;
        private readonly IMapper _mapper;

        public NotificationService(EmployeeDBContext context, IMapper mapper, ILogger<NotificationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Create Notification
        public async Task<bool> AddNotificationAsync(CreateNotificationRequest notification)
        {
            try
            {
                Notification mappedNotification = _mapper.Map<Notification>(notification);
                await _context.AddAsync(mappedNotification);
                await SaveAsync();

                _logger.LogInformation($"Notification added successfully. Id: {mappedNotification.Id}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add notification: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }

        #endregion

        #region Get Employee Notifications
        public async Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync(int employeeId, EmployeeType employeeType)
        {
            try
            {
                switch (employeeType)
                {
                    case EmployeeType.Employee:
                        return await _context.Notifications.Where(e => e.AssignedToId == employeeId && e.IsSeen == false).ToListAsync();

                    case EmployeeType.Admin:
                        return await _context.Notifications.Where(e => e.AssignedById == employeeId && e.IsSeen == false).ToListAsync();

                    default:
                        throw new Exception("Notifications not available for the specified employee type.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve notifications: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }

        #endregion

        #region Mark Notification as Read

        public async Task<bool> MarkNotificationAsReadAsync(Notification notification)
        {
            try
            {
                if (!notification.IsSeen)
                {
                    notification.IsSeen = true;
                    _context.Update(notification);
                    await SaveAsync();

                    _logger.LogInformation($"Notification marked as read. Id: {notification.Id}");

                    return true;
                }
                else
                {
                    throw new Exception("Notification is already marked as read.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to mark notification as read: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }

        #endregion

        #region Notifications Exists
        public async Task<bool> NotificationsExistsAsync(MarkNotificationsAsSeenRequest notifications)
        {
            var notificationIds = notifications.Ids;
            var existingIds = await _context.Notifications.Where(n => notificationIds.Contains(n.Id)).Select(n => n.Id).ToListAsync();
            return existingIds.Count == notificationIds.Count;
        }
        #endregion

        #region Save Async

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save changes: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }

        #endregion

        #region Get Employee Notification
        public async Task<Notification?> GetEmployeeNotificationAsync(int notificationId)
        {
            try
            {
                return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve notification: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }
        #endregion

        #region Mark Notifications As Read
        public async Task<IEnumerable<int>> MarkNotificationsAsReadAsync(MarkNotificationsAsSeenRequest notifications)
        {
            _logger.LogInformation("Marking notifications as read...");
            List<int> ids = notifications.Ids;
            List<Notification> existingNotifications = await _context.Notifications.Where(n => ids.Contains(n.Id)).ToListAsync();

            List<int> seenNofications = [];
            foreach (Notification notification in existingNotifications)
            {
                if (notification.IsSeen)
                {
                    seenNofications.Add(notification.Id);
                    _logger.LogInformation($"Notification ID {notification.Id} is already marked as seen.");

                }
                else
                {
                    _logger.LogInformation($"Marking notification ID {notification.Id} as seen.");
                    notification.IsSeen = true;
                }
            }
            await SaveAsync();
            _logger.LogInformation("Notifications marked as read successfully.");
            return seenNofications;
        }
        #endregion
    }
}

