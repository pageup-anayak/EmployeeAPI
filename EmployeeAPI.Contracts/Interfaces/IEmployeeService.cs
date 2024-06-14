using EmployeeAPI.Contracts.Dtos.Requests.Employees;
using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contracts.Interfaces
{
    public interface IEmployeeService
    {
        // SuperAdmin can only create Admin
        // Employee can create by themself
        Task<bool> CreateEmployeeAsync(CreateEmployeeRequestDTO employee);
        Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequestDTO employee);
        // Admin can delete Employee
        // SuperAdmin can delete Admin and Employee
        Task<bool> DeleteEmployeeAsync(Employee employee);
        Task<bool> SaveAsync();
        // can access SuperAdmin 
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        // can access by Admin and SuperAdmin
        // for Admin will get all employees of his department
        // for SuperAdmin will get all employees
        Task<IEnumerable<Employee>> GetEmployees();
        /// <summary>
        /// checks whether employees exists on the basis of id or email as they are unique
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> EmployeeExists(int? employeeId, string? email);
        Task<Employee> GetEmployee(int? employeeId, string? email);
    }
}
