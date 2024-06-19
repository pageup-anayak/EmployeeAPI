using EmployeeAPI.Contracts.Dtos.Requests.Departments;
using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contracts.Interfaces
{
    public interface IDepartmentService
    {
        // SuperAdmin can only access 
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        // SuperAdmin can only access 
        Task<bool> AddDepartmentAsync(CreateDepartmentRequest department);
        // SuperAdmin can only access 
        Task<bool> UpdateDepartmentAsync(Department department);
        // SuperAdmin can only access 
        Task<bool> DeleteDepartmentAsync(Department department);
        Task<Department?> GetDepartmentAsync(int? departmentId, string? departmentName);
        Task<bool> SaveAsync();
        Task<bool> DepartmentExistsAsync(int? departmentId, string? departmentName);
        Task<bool> DepartmentHasAdmin(int adminId, int departmentId);
    }
}
