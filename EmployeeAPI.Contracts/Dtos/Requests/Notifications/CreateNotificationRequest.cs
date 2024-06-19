using EmployeeAPI.Contracts.Enums;
namespace EmployeeAPI.Contracts.Dtos.Requests.Notifications
{
    public class CreateNotificationRequest
    {
        public required NotificationMessage notificationMessage { get; set; }
        public required bool IsSeen { get; set; }
        public required int AssignedToId { get; set; }
        public required int AssignedById { get; set; }
        public required int TodoId { get; set; }
    }
}
