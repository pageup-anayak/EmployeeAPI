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
                        return await _context.Notifications.Where(e => e.AssignedToId == employeeId).ToListAsync();

                    case EmployeeType.Admin:
                        return await _context.Notifications.Where(e => e.AssignedById == employeeId).ToListAsync();

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
    }
}

