using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class TodoService : ITodoService
    {
        private readonly EmployeeDBContext _context;
        private readonly ILogger<ITodoService> _logger;
        public TodoService(EmployeeDBContext context, ILogger<ITodoService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<bool> AddTodoAsync(Todo todo)
        {
            _context.Add(todo);
            return SaveAsync();
        }

        public Task<bool> DeleteTodoAsync(Todo todo)
        {
            _context.Remove(todo);
            return SaveAsync();
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync(TaskStatus taskStatus)
        {
            return await _context.Todos.ToArrayAsync();
        }

        public async Task<Todo> GetTodoByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public Task<IEnumerable<Todo>> GetTodosByEmployeeIdAsync(int employeeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> TodoExists(int todoId)
        {
            var todo = await _context.Todos.FindAsync(todoId);
            return todo != null;
        }

        public async Task<bool> UpdateTodoAsync(Todo todo)
        {
            _context.Update(todo);
            return await SaveAsync();
        }
    }

}

