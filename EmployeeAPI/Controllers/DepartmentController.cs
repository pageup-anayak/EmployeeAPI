using EmployeeAPI.Contracts.Dtos.Requests.Departments;
using EmployeeAPI.Contracts.Interfaces;
using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService,
                                    IEmployeeService employeeService,
                                    ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
            _employeeService = employeeService;
        }

        #region Get All Departments
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Department>))]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDepartments()
        {
            Task<IEnumerable<Department>> departments = _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }
        #endregion

        #region Create Department
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(IEnumerable<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDepartmentAsync(CreateDepartmentRequest department)
        {
            if ((await _departmentService.DepartmentExistsAsync(null, department.Name)))
            {
                return BadRequest("Department name already exists");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var created = _departmentService.AddDepartmentAsync(department);
            return Ok(created);
        }
        #endregion

        #region Get Department
        /// <summary>
        /// </summary>
        /// <param name="identifier"></param> could be id or name
        /// <returns></returns>
        [HttpGet("{identifier}")]
        [Authorize]
        public IActionResult GetDepartment(string identifier)
        {
            Task<Department?> department;
            if (int.TryParse(identifier, out var departmentId))
            {
                department = _departmentService.GetDepartmentAsync(departmentId, null);
            }
            else
            {
                department = _departmentService.GetDepartmentAsync(null, identifier);
            }
            return department == null ? NotFound() : Ok(department);

        }
        #endregion

        #region Update Department
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<bool>))]
        public async Task<IActionResult> UpdateDepartment(int id, Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(await _departmentService.DepartmentExistsAsync(id, null)))
            {
                return BadRequest("Department dose not exists");
            }
            if (!(await _departmentService.DepartmentExistsAsync(id, department.Name)))
            {
                return BadRequest("Name already taken");
            }
            var updated = await _departmentService.UpdateDepartmentAsync(department);
            return Ok(updated);
        }
        #endregion

        #region Delete Department
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<bool>))]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (!await _departmentService.DepartmentExistsAsync(id, null))
            {
                return BadRequest("Department dose not exists");
            }
            // Whether have associated employees in department or not.
            var dept = await _departmentService.GetDepartmentAsync(id, null);
            // have no employee associated with department
            if (dept != null && (dept.Employees == null || dept.Employees.Count == 0))
            {
                var deletedDept = await _departmentService.DeleteDepartmentAsync(dept);
                return Ok(deletedDept);
            }
            return BadRequest("Have associated employees");
        }
        #endregion
    }
}
