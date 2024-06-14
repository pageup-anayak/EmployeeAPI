using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmployeeAPI.Contracts.enums;

namespace EmployeeAPI.Provider.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly EmployeeDBContext _context;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(EmployeeDBContext context, ILogger<DepartmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddDepartmentAsync(Department department)
        {
            _logger.LogInformation($"Adding department: {department.Name}");
            _context.Add(department);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully added department: {department.Name}");
            }
            else
            {
                _logger.LogError($"Failed to add department: {department.Name}");
            }
            return result;
        }

        public async Task<bool> DepartmentExistsAsync(int? departmentId, string? departmentName)
        {
            _logger.LogInformation($"Checking if department exists by ID: {departmentId} or Name: {departmentName}");

            Department? department1 = null;
            Department? department2 = null;

            if (departmentId != null)
            {
                department1 = await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
            }
            if (departmentName != null)
            {
                department2 = await _context.Departments.FirstOrDefaultAsync(d => d.Name == departmentName);
            }
            bool exists = department1 != null || department2 != null;
            _logger.LogInformation($"Department exists: {exists}");

            if (department1 != null && department2 != null)
            {
                return department1.Id == department2.Id;
            }
            return exists;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            _logger.LogInformation("Retrieving all departments");
            return await _context.Departments.ToArrayAsync();
        }

        public async Task<Department?> GetDepartmentAsync(int? departmentId, string? departmentName)
        {
            _logger.LogInformation($"Retrieving department by ID: {departmentId} or Name: {departmentName}");

            if (departmentId == null && departmentName == null)
            {
                _logger.LogError("Either departmentId or departmentName must be provided");
                throw new Exception("Either departmentId or departmentName must be provided");
            }
            if (departmentId != null)
            {
                return await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
            }
            return await _context.Departments.FirstOrDefaultAsync(d => d.Name == departmentName);
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved {saved} changes to the database");
            return saved > 0;
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            _logger.LogInformation($"Updating department: {department.Name}");
            _context.Update(department);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully updated department: {department.Name}");
            }
            else
            {
                _logger.LogError($"Failed to update department: {department.Name}");
            }
            return result;
        }

        public async Task<bool> DeleteDepartmentAsync(Department department)
        {
            _logger.LogInformation($"Deleting department: {department.Name}");
            _context.Remove(department);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully deleted department: {department.Name}");
            }
            else
            {
                _logger.LogError($"Failed to delete department: {department.Name}");
            }
            return result;
        }

        public async Task<bool> DepartmentHasAdmin(int adminId, int departmentId)
        {
            var hasAdmin = await _context.Departments.FirstOrDefaultAsync((d) => d.Id == departmentId && d.Employees != null && d.Employees.Any(e => e.Id == adminId && e.EmployeeType == EmployeeType.Admin));

            return hasAdmin != null;
        }
    }
}

