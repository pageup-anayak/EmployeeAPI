using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contracts.Interfaces
{
    public interface ITodoService
    {
        // can accessed by Employee to whom task is assigned, Admin on have assigned task, SuperAdmin
        Task<Todo> GetTodoByIdAsync(int id);
        // Admin -> get all task he has assigned 
        // SuperAdmin -> get all the task
        // Employee -> get all he got assigned 
        Task<IEnumerable<Todo>> GetAllTodosAsync(TaskStatus taskStatus);
        // Admin and SuperAdmin can create Task and can ony be assigned to Employee
        // Admin can assign task to his dept. employees
        Task<bool> AddTodoAsync(Todo todo);
        // Admin can update title, description
        Task<bool> UpdateTodoAsync(Todo todo);
        // Admin and SuperAdmin can delete the task
        Task<bool> DeleteTodoAsync(Todo todo);
        // admin will get all the task which he assigned to that employeeId
        // Superadmin will get all the task which he assigned to that employeeId
        Task<IEnumerable<Todo>> GetTodosByEmployeeIdAsync(int employeeId);
        Task<bool> SaveAsync();
        Task<bool> TodoExists(int todoId);
    }
}
