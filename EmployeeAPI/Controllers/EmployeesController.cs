using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Dtos;
using Microsoft.EntityFrameworkCore.Update.Internal;
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

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public IActionResult GetEmployees()
        {
            Task<IEnumerable<Employee>> employees = _employeeService.GetEmployees();
            return Ok(employees);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public IActionResult GetEmployee(int id)
        {
            var employee = _employeeService.GetEmployee(id, null);
            return Ok(employee);
        }

        [HttpGet("/department/{departmentId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Employee>))]
        public async Task<IActionResult> GetEmployeesByDepartmentAsync(int departmentId)
        {
            if (!(await _departmentService.DepartmentExistsAsync(departmentId, null)))
            {
                return BadRequest("Department not exists");
            }
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            return Ok(employees);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> CreateEmployee(UpdateEmployeeRequestDTO employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(await _employeeService.EmployeeExists(null, employee.Email)))
            {
                return BadRequest("Employee already exists");
            }
            var created = await _employeeService.CreateEmployeeAsync(employee);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(await _employeeService.EmployeeExists(id, null)))
            {
                return BadRequest("Employee dose not exists");
            }

            if (!(await _employeeService.EmployeeExists(id, employee.Email)))
            {
                return BadRequest("Invalid email");
            }
            switch (employee.EmployeeType)
            {
                case EmployeeType.Admin:
                    {
                        // department can have only one admin
                        if ((await _departmentService.DepartmentHasAdmin(employee.Id, id)))
                        {
                            return BadRequest("Department can have only one Admin");
                        }
                        break;
                    }

                case EmployeeType.SuperAdmin:
                    {
                        break;
                    }

                case EmployeeType.Employee:
                    {
                        break;
                    }
            }
            var created = await _employeeService.UpdateEmployeeAsync(employee);
            return Ok(created);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!(await _employeeService.EmployeeExists(id, null)))
            {
                return BadRequest("Employee dose not exists");
            }
            // TODO: make to delete Todos and Notifications. 
            var deleted = await _employeeService.DeleteEmployeeAsync(await _employeeService.GetEmployee(id, null));
            return Ok(deleted);
        }
    }
}
