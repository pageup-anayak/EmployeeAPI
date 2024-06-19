using EmployeeAPI.Contracts.Dtos.Requests.Todos;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Models;

namespace EmployeeAPI.Contracts.Interfaces
{
    public interface ITodoService
    {
        // can accessed by Employee to whom task is assigned, Admin on have assigned task, SuperAdmin
        Task<Todo?> GetTodoByIdAsync(int id);
        // Admin -> get all task he has assigned 
        // SuperAdmin -> get all the task
        // Employee -> get all he got assigned 
        Task<List<Todo>> GetAllTodosAsync(TodoStatus? taskStatus);
        // Admin and SuperAdmin can create Task and can ony be assigned to Employee
        // Admin can assign task to his dept. employees
        Task<bool> AddTodoAsync(CreateTodoRequest todo);
        // Admin can update title, description
        Task<bool> UpdateTodoAsync(Todo todo);
        // Admin and SuperAdmin can delete the task
        Task<bool> DeleteTodoAsync(Todo todo);
        // admin will get all the task which he assigned to that employeeId
        // Superadmin will get all the task which he assigned to that employeeId
        Task<List<Todo>> GetTodosByEmployeeIdAsync(int employeeId);
        Task<bool> SaveAsync();
        Task<bool> TodoExists(int todoId);
    }
}
