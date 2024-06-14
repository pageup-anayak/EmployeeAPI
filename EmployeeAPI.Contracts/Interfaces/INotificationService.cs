using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EmployeeAPI.Contracts.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        // Notification can only create when there change in status of task
        Task<Notification> AddNotificationAsync(Notification notification);
        // Only Status of notification will change
        Task<Notification> UpdateNotificationAsync(Notification notification);
        // Employee will get there notifications
        Task<IEnumerable<Notification>> GetNotificationsForEmployeeAsync(int employeeId);
    }
}
