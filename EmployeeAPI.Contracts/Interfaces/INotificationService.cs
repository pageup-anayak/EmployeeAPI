using EmployeeAPI.Contracts.Dtos.Requests.Notifications;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Models;

namespace EmployeeAPI.Contracts.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync(int employeeId, EmployeeType employeeType);
        // Notification can only create when there change in status of task
        Task<bool> AddNotificationAsync(CreateNotificationRequest notification);
        // Only Status of notification will change
        Task<bool> MarkNotificationAsReadAsync(Notification notification);

        Task<Notification?> GetEmployeeNotificationAsync(int notificationId);
        Task<bool> SaveAsync();
        Task<bool> NotificationsExistsAsync(MarkNotificationsAsSeenRequest notifications);
        Task<IEnumerable<int>> MarkNotificationsAsReadAsync(MarkNotificationsAsSeenRequest notifications);

    }
}
