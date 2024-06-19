using EmployeeAPI.Contracts.enums;

namespace EmployeeAPI.Contracts.Dtos.Requests.Todos
{
    public class UpdateTodoRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TodoStatus? Status { get; set; }
        public int? AssignedToId { get; set; }
        public int? AssignedById { get; set; }

    }
}
