using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmployeeAPI.Contracts.Dtos.Requests.Todos;
using AutoMapper;

namespace EmployeeAPI.Provider.Services
{
    public class TodoService : ITodoService
    {
        private readonly EmployeeDBContext _context;
        private readonly ILogger<ITodoService> _logger;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public TodoService(EmployeeDBContext context,
                           ILogger<ITodoService> logger,
                           IMapper mapper,
                           INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        #region AddTodoAsync
        public async Task<bool> AddTodoAsync(CreateTodoRequest todo)
        {
            _logger.LogInformation("Adding a new Todo with title: {Title}", todo.Title);
            var mappedTodo = _mapper.Map<Todo>(todo);
            _context.Add(mappedTodo);
            var result = await SaveAsync();
            //mappedNotification.Message = NotificationMessageExtensions.GetMessage(NotificationMessage.TaskAssigned, notification.AssignedToId, notification.AssignedToId, notification.TodoId);
            //int assignedById = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType);
            //CreateNotificationRequest nReq = new CreateNotificationRequest(NotificationMessage.TaskAssigned, true, todo.AssignedToId,id);
            //_notificationService.AddNotificationAsync();
            //Notification notification = new Notification();
            _logger.LogInformation("Todo with title: {Title} added successfully", todo.Title);
            return result;
        }
        #endregion

        #region DeleteTodoAsync
        public async Task<bool> DeleteTodoAsync(Todo todo)
        {
            _logger.LogInformation("Deleting Todo with ID: {TodoId}", todo.Id);
            _context.Remove(todo);
            var result = await SaveAsync();
            _logger.LogInformation("Todo with title: {Title} added successfully", todo.Title);
            return result;
        }
        #endregion

        #region GetTodosAsync
        public async Task<List<Todo>> GetAllTodosAsync(TodoStatus? todoStatus)
        {
            _logger.LogInformation("Retrieving all Todos with status: {TodoStatus}", todoStatus);
            List<Todo> todos;
            if (todoStatus != null)
            {
                todos = await _context.Todos.Where(t => t.Status == todoStatus).ToListAsync();
            }
            todos = await _context.Todos.ToListAsync();
            _logger.LogInformation("{Count} Todos retrieved", todos.Count);
            return todos;
        }
        #endregion

        #region GetTodoById
        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving Todo with ID: {TodoId}", id);
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                _logger.LogWarning("Todo with ID: {TodoId} not found", id);
            }
            else
            {
                _logger.LogInformation("Todo with ID: {TodoId} retrieved successfully", id);
            }
            return todo;
        }
        #endregion

        #region GetTodosByEmployeeId
        public async Task<List<Todo>> GetTodosByEmployeeIdAsync(int employeeId)
        {
            _logger.LogInformation("Retrieving Todos for Employee with ID: {EmployeeId}", employeeId);
            Employee employee = (await _context.Employees.FindAsync(employeeId))!;
            EmployeeType employeeType = employee.EmployeeType;
            List<Todo> todos;
            switch (employeeType)
            {
                case EmployeeType.Admin:
                    {
                        _logger.LogInformation("Employee with ID: {EmployeeId} is an Admin, retrieving Todos assigned by them", employeeId);
                        todos = await _context.Todos.Where(t => t.AssignedById == employeeId).ToListAsync();
                        break;
                    }
                case EmployeeType.Employee:
                    {
                        _logger.LogInformation("Employee with ID: {EmployeeId} is an Employee, retrieving Todos assigned to them", employeeId);
                        todos = await _context.Todos.Where(t => t.AssignedToId == employeeId).ToListAsync();
                        break;
                    }
                default:
                    {
                        _logger.LogError("Invalid EmployeeType for Employee with ID: {EmployeeId}", employeeId);
                        throw new Exception("SuperAdmin not allowed");
                    }
            }
            _logger.LogInformation("{Count} Todos retrieved for Employee with ID: {EmployeeId}", todos.Count, employeeId);
            return todos;
        }
        #endregion

        #region SaveAsync
        public async Task<bool> SaveAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            var saved = await _context.SaveChangesAsync();
            _logger.LogInformation("{Changes} changes saved to the database", saved);
            return saved > 0;
        }
        #endregion

        #region TodoExists
        public async Task<bool> TodoExists(int todoId)
        {
            _logger.LogInformation("Checking existence of Todo with ID: {TodoId}", todoId);
            var todo = await _context.Todos.FindAsync(todoId);
            bool exists = todo != null;
            _logger.LogInformation("Todo with ID: {TodoId} exists: {Exists}", todoId, exists);
            return exists;
        }
        #endregion

        #region UpdateTodo
        public async Task<bool> UpdateTodoAsync(Todo todo)
        {
            _logger.LogInformation("Updating Todo with ID: {TodoId}", todo.Id);
            _context.Update(todo);
            var res = await SaveAsync();
            _logger.LogInformation("Todo with ID: {TodoId} updated successfully", todo.Id);
            return res;
        }
        #endregion
    }
}

