using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Dtos.Requests.Employees;

namespace EmployeeAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeesController(IEmployeeService employeeService,
                                   ILogger<EmployeesController> logger,
                                   IDepartmentService departmentService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        #region Get Employees
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public async Task<IActionResult> GetEmployees()
        {
            _logger.LogInformation("Getting all employees");
            var employees = await _employeeService.GetEmployees();
            _logger.LogInformation("Successfully retrieved employees");
            return Ok(employees);
        }
        #endregion

        #region Get Employee
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        public async Task<IActionResult> GetEmployee(int id)
        {
            _logger.LogInformation("Getting employee with ID: {id}", id);
            var employee = await _employeeService.GetEmployee(id, null);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID: {id} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved employee with ID: {id}", id);
            return Ok(employee);
        }
        #endregion

        #region Get Employees By Department 
        [HttpGet("department/{departmentId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public async Task<IActionResult> GetEmployeesByDepartmentAsync(int departmentId)
        {
            _logger.LogInformation("Getting employees for department ID: {departmentId}", departmentId);
            if (!(await _departmentService.DepartmentExistsAsync(departmentId, null)))
            {
                _logger.LogWarning("Department with ID: {departmentId} does not exist", departmentId);
                return BadRequest("Department not exists");
            }
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            _logger.LogInformation("Successfully retrieved employees for department ID: {departmentId}", departmentId);
            return Ok(employees);
        }
        #endregion

        #region Create Employee
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeRequestDTO employee)
        {
            _logger.LogInformation("Creating a new employee with email: {email}", employee.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for employee creation");
                return BadRequest(ModelState);
            }
            if (await _employeeService.EmployeeExists(null, employee.Email))
            {
                _logger.LogWarning("Employee with email: {email} already exists", employee.Email);
                return BadRequest("Employee already exists");
            }
            var created = await _employeeService.CreateEmployeeAsync(employee);
            _logger.LogInformation("Successfully created employee with email: {email}", employee.Email);
            return Ok(created);
        }
        #endregion

        #region Update Employee
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequestDTO employee)
        {
            _logger.LogInformation("Updating employee with ID: {id}", id);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for employee update");
                return BadRequest(ModelState);
            }
            if (!await _employeeService.EmployeeExists(id, null))
            {
                _logger.LogWarning("Employee with ID: {id} does not exist", id);
                return BadRequest("Employee does not exist");
            }
            if (!await _employeeService.EmployeeExists(id, employee.Email))
            {
                _logger.LogWarning("Invalid email for employee ID: {id}", id);
                return BadRequest("Invalid email");
            }
            switch (employee.EmployeeType)
            {
                case EmployeeType.Admin:
                    {
                        _logger.LogInformation("Checking if department ID: {departmentId} has an admin for employee ID: {id}", employee.DepartmentId, id);
                        if (await _departmentService.DepartmentHasAdmin(id, employee.DepartmentId))
                        {
                            _logger.LogWarning("Department ID: {departmentId} already has an admin", employee.DepartmentId);
                            return BadRequest("Department can have only one Admin");
                        }
                        break;
                    }
                case EmployeeType.SuperAdmin:
                case EmployeeType.Employee:
                    break;
            }
            var updated = await _employeeService.UpdateEmployeeAsync(employee);
            _logger.LogInformation("Successfully updated employee with ID: {id}", id);
            return Ok(updated);
        }
        #endregion

        #region Delete Employee
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {id}", id);
            if (!await _employeeService.EmployeeExists(id, null))
            {
                _logger.LogWarning("Employee with ID: {id} does not exist", id);
                return BadRequest("Employee does not exist");
            }
            // TODO: make to delete Todos and Notifications.
            var employee = await _employeeService.GetEmployee(id, null);
            var deleted = await _employeeService.DeleteEmployeeAsync(employee);
            _logger.LogInformation("Successfully deleted employee with ID: {id}", id);
            return Ok(deleted);
        }
        #endregion
    }
}

