using EmployeeAPI.Contracts.Enums;

namespace EmployeeAPI.Extensions.Enums
{
    public static class NotificationMessageExtensions
    {
        public static string GetMessage(this NotificationMessage notificationMessage, int assignedById, int assignedToId, int todoId)
        {
            return notificationMessage switch
            {
                NotificationMessage.TaskAssigned => $"You have been assigned {todoId} task by {assignedById}",
                NotificationMessage.TaskCompleted => $"{assignedToId} have completed task {todoId}",
                NotificationMessage.TaskContentUpdated => $"{assignedById} have updated task {todoId}",
                NotificationMessage.TaskRejected => $"{assignedById} have rejected task {todoId}",
                NotificationMessage.TaskApproved => $"{assignedById} have approved task {todoId}",
                NotificationMessage.TaskInProgress => $"Task {todoId} is in progress by {assignedToId}",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
