using EmployeeAPI.Contracts.enums;

namespace EmployeeAPI.Contracts.Enums
{
    public static class TodoStatusExtensions
    {
        private static readonly Dictionary<TodoStatus, List<TodoStatus>> _allowedTransitions = new Dictionary<TodoStatus, List<TodoStatus>> {
            {TodoStatus.NotStarted, new List<TodoStatus> {TodoStatus.InProgress} },
            {TodoStatus.InProgress, new List<TodoStatus> {TodoStatus.Completed} },
            {TodoStatus.Completed, new List<TodoStatus> { TodoStatus.Rejected,TodoStatus.Approved} },
            {TodoStatus.Approved, new List<TodoStatus> () },
            {TodoStatus.Rejected, new List<TodoStatus> () },
        };

        public static bool CanTransitionTo(this TodoStatus currentStatus, TodoStatus newStatus)
        {

            return _allowedTransitions.ContainsKey(currentStatus) && _allowedTransitions[currentStatus].Contains(newStatus);
        }
    }
}
