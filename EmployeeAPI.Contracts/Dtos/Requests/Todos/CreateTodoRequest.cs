
namespace EmployeeAPI.Contracts.Dtos.Requests.Todos
{
    public class CreateTodoRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int AssignedToId { get; set; }

        public void Deconstruct(out string title, out string description, out int assignedToId)
        {
            title = Title;
            description = Description;
            assignedToId = AssignedToId;
        }

        public void Deconstruct(out object title, out object Item2)
        {
            throw new NotImplementedException();
        }
    }
}
