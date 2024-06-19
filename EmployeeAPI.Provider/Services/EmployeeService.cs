using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmployeeAPI.Contracts.Dtos.Requests.Employees;
using AutoMapper;

namespace EmployeeAPI.Provider.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDBContext _context;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IMapper _mapper;

        public EmployeeService(EmployeeDBContext context, ILogger<EmployeeService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        #region CreateEmployeeAsync
        public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequestDTO employee)
        {
            _logger.LogInformation($"Creating employee: {employee.Name}");
            var mappedEmployee = _mapper.Map<Employee>(employee);
            _context.Add(mappedEmployee);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully created mappedEmployee: {mappedEmployee.Name}");
            }
            else
            {
                _logger.LogError($"Failed to create mappedEmployee: {mappedEmployee.Name}");
            }
            return result;
        }
        #endregion

        #region DeleteEmployeeAsync
        public async Task<bool> DeleteEmployeeAsync(Employee employee)
        {
            _logger.LogInformation($"Deleting employee: {employee.Name}");
            _context.Employees.Remove(employee);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully deleted employee: {employee.Name}");
            }
            else
            {
                _logger.LogError($"Failed to delete employee: {employee.Name}");
            }
            return result;
        }
        #endregion

        #region EmployeeExists
        public async Task<bool> EmployeeExists(int? employeeId, string? email)
        {
            _logger.LogInformation($"Checking if employee exists by ID: {employeeId} or Email: {email}");

            Employee? employee1 = null;
            Employee? employee2 = null;

            if (employeeId != null)
            {
                employee1 = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            }
            if (email != null)
            {
                employee2 = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            }
            bool exists = employee1 != null || employee2 != null;
            _logger.LogInformation($"Employee exists: {exists}");

            if (employee1 != null && employee2 != null)
            {
                return employee1.Id == employee2.Id;
            }
            return exists;
        }
        #endregion

        #region GetEmployee
        public async Task<Employee> GetEmployee(int? employeeId, string? email)
        {
            _logger.LogInformation($"Retrieving employee by ID: {employeeId} or Email: {email}");

            if (employeeId == null && email == null)
            {
                _logger.LogError("Either employeeId or email must be provided");
                throw new Exception("Either employeeId or email must be provided");
            }
            if (employeeId != null)
            {
                return await _context.Employees.FirstAsync(e => e.Id == employeeId);
            }
            return await _context.Employees.FirstAsync(e => e.Email == email);
        }
        #endregion

        #region GetEmployees
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            _logger.LogInformation("Retrieving all employees");
            return await _context.Employees.ToListAsync();
        }
        #endregion

        #region GetEmployeeByDepartment
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            _logger.LogInformation($"Retrieving employees by department ID: {departmentId}");
            return await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
        }
        #endregion

        #region SaveAsync
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved {saved} changes to the database");
            return saved > 0;
        }
        #endregion

        #region UpdateEmployeeAsync
        public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequestDTO employee)
        {
            _logger.LogInformation($"Updating employee: {employee.Name}");
            var mappedEmployee = _mapper.Map<Employee>(employee);
            _context.Employees.Update(mappedEmployee);
            var result = await SaveAsync();
            if (result)
            {
                _logger.LogInformation($"Successfully updated employee: {mappedEmployee.Name}");
            }
            else
            {
                _logger.LogError($"Failed to update employee: {mappedEmployee.Name}");
            }
            return result;
        }
        #endregion
    }
}

