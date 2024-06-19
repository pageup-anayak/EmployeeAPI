using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Enums;
using EmployeeAPI.Contracts.Dtos.Requests.Todos;
using EmployeeAPI.Contracts.Interfaces;

namespace EmployeeAPI.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly ITodoService _todoService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        public TodoController(IEmployeeService employeeService,
                              IDepartmentService departmentService,
                              ITodoService todoService,
                              ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _logger = logger;
        }

        #region Get Todo
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        public async Task<IActionResult> GetTodo(int id)
        {
            if (!await _todoService.TodoExists(id))
            {
                return BadRequest("Todo dose not exists");
            }
            var todo = await _todoService.GetTodoByIdAsync(id);
            return Ok(todo);
        }
        #endregion

        //TODO: update below method to have TodoStatus as well
        #region Get Todos 
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Todo>))]
        public async Task<IActionResult> GetTodos()
        {
            // extract out the user details from the token
            // id, role, name

            var empId = GetUserIdFromToken();
            EmployeeType role = GetUserRoleFromToken();
            List<Todo> todos;
            switch (role)
            {
                // returns todos assigned by admin
                case EmployeeType.Admin:
                    {
                        todos = await _todoService.GetTodosByEmployeeIdAsync(int.Parse(empId));
                        break;
                    }
                // returns all todos 
                case EmployeeType.SuperAdmin:
                    {
                        todos = await _todoService.GetAllTodosAsync(null);
                        break;
                    }
                // returns todos assigned to employee
                default:
                    {
                        todos = await _todoService.GetTodosByEmployeeIdAsync(int.Parse(empId));
                        break;
                    }
            }
            return Ok(todos);
        }
        #endregion

        #region Get Todos By EmployeeId
        /// <summary>
        /// if(employeeId == adminId) get all the todos assigned by him -> only superadmin can access
        /// if(employeeId == employeeId) get all the todos assigned to him -> superadmin and admin of department of which employee beloging to.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet("/employee/{employeeId}/{employeeType}")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> getTodosByEmployeeId(int employeeId)
        {
            var employee = await _employeeService.GetEmployee(employeeId, null);
            if (employee == null)
            {
                return BadRequest("Employee dose not exists");
            }
            var role = GetUserRoleFromToken();
            var employeeRole = employee.EmployeeType;
            if (role == EmployeeType.Admin)
            {
                // Logic for Admin role
                Console.WriteLine("Role is Admin");

                if (employeeRole == EmployeeType.SuperAdmin)
                {
                    // Logic when employeeRole is SuperAdmin
                    return BadRequest("Admin can access SuperAdmin data");
                }
                else if (employeeRole == EmployeeType.Admin)
                {
                    // Logic when employeeRole is Admin
                    return BadRequest("Admin can access other admin data");
                }
                else
                {
                    // Logic when employeeRole is Employee
                    var adminId = GetUserIdFromToken();
                    var admin = (await _employeeService.GetEmployee(int.Parse(adminId), null))!;
                    if (admin.DepartmentId != employee.DepartmentId)
                    {
                        return BadRequest("Admin can not access other department emloyees task");
                    }
                    else
                    {
                        return Ok(await _todoService.GetTodosByEmployeeIdAsync(employeeId));
                    }
                }
            }
            else
            {
                // Logic for SuperAdmin role
                Console.WriteLine("Role is SuperAdmin");

                if (employeeRole == EmployeeType.SuperAdmin)
                {
                    // Logic when employeeRole is SuperAdmin
                    return BadRequest("SuperAdmin can not access the SuperAdmin");
                }
                else if (employeeRole == EmployeeType.Admin)
                {
                    // Logic when employeeRole is Admin
                    return Ok(await _todoService.GetTodosByEmployeeIdAsync(employeeId));
                }
                else
                {
                    // Logic when employeeRole is Employee
                    return Ok(await _todoService.GetTodosByEmployeeIdAsync(employeeId));
                }
            }
        }
        #endregion

        #region Update Todo
        [HttpPost("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        //[Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest todoUpdateDto)
        {
            Todo? todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return BadRequest("Todo dose not exists");
            }
            var role = GetUserRoleFromToken();
            List<string> res = [];
            switch (role)
            {
                case EmployeeType.Admin:
                    {
                        // Update properties based on non-null values in DTO
                        if (todoUpdateDto.Title != null)
                        {
                            todo.Title = todoUpdateDto.Title;
                            res.Add("Updated title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            todo.Description = todoUpdateDto.Description;
                            res.Add("Updated description");
                        }

                        if (todoUpdateDto.Status != null)
                        {
                            if ((todoUpdateDto.Status.Value == TodoStatus.Approved || todoUpdateDto.Status.Value == TodoStatus.Rejected) && todo.Status == TodoStatus.Completed)
                            {
                                todo.Status = todoUpdateDto.Status.Value;
                                res.Add("Updated status");
                            }
                            else
                            {
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }
                        }

                        if (todoUpdateDto.AssignedToId != null)
                        {
                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedToId, null);
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedToId, null))
                            {
                                return BadRequest("Employee dose not exists");
                            }
                            if (employee.EmployeeType != EmployeeType.Employee)
                            {
                                return BadRequest($"Can not assign task to {employee.EmployeeType}");
                            }

                            todo.AssignedToId = (int)todoUpdateDto.AssignedToId;
                            res.Add("Updated AssignedTo");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            return BadRequest("Admin can not change assign to");
                        }
                        break;
                    }
                case EmployeeType.SuperAdmin:
                    {
                        // Update properties based on non-null values in DTO
                        if (todoUpdateDto.Title != null)
                        {
                            todo.Title = todoUpdateDto.Title;
                            res.Add("Updated title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            todo.Description = todoUpdateDto.Description;
                            res.Add("Updated description");
                        }

                        if (todoUpdateDto.Status != null)
                        {
                            if ((todoUpdateDto.Status.Value == TodoStatus.Approved || todoUpdateDto.Status.Value == TodoStatus.Rejected) && todo.Status == TodoStatus.Completed)
                            {
                                todo.Status = todoUpdateDto.Status.Value;
                                res.Add("Updated status");
                            }
                            else
                            {
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }
                        }

                        if (todoUpdateDto.AssignedToId != null)
                        {
                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedToId, null);
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedToId, null))
                            {
                                return BadRequest("Employee dose not exists");
                            }
                            if (employee.EmployeeType != EmployeeType.Employee)
                            {
                                return BadRequest($"Can not assign task to {employee.EmployeeType}");
                            }

                            todo.AssignedToId = (int)todoUpdateDto.AssignedToId;
                            res.Add("Updated AssignedTo");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedById, null))
                            {
                                return BadRequest("Employee dose not exists");
                            }
                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedToId, null);
                            if (employee.EmployeeType != EmployeeType.Admin)
                            {
                                return BadRequest($"Can not assign task to {employee.EmployeeType}");
                            }
                            todo.AssignedById = (int)todoUpdateDto.AssignedById;
                        }
                        break;
                    }
                default:
                    {
                        // Update properties based on non-null values in DTO
                        if (todoUpdateDto.Status != null)
                        {
                            if (todoUpdateDto.Status == TodoStatus.Approved)
                            {
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }
                            if (TodoStatusExtensions.CanTransitionTo(todo.Status, (TodoStatus)todoUpdateDto.Status))
                            {
                                todo.Status = (TodoStatus)todoUpdateDto.Status;
                                res.Add("Updated status");
                            }
                        }

                        if (todoUpdateDto.Title != null)
                        {
                            return BadRequest("Employee can updated title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            return BadRequest("Employee can updated description");
                        }


                        if (todoUpdateDto.AssignedToId != null)
                        {
                            return BadRequest("Employee can not assign task");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            return BadRequest("Employee can not assign task");
                        }
                        break;
                    }
            }

            if (res.Count == 0)
            {
                res.Add("Nothing updated");
            }
            return Ok(res);
        }
        #endregion

        #region Create Todo
        [HttpPost]
        public async Task<IActionResult> CreateTodoAsync([FromBody] CreateTodoRequest createTodo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (title, description, assignedToId) = createTodo;

            var emp1 = await _employeeService.GetEmployee(assignedToId, null);
            if (emp1.EmployeeType != EmployeeType.Employee)
            {
                return BadRequest($"Can assign to {emp1.EmployeeType}");
            }
            return Ok(await _todoService.AddTodoAsync(createTodo));
        }
        #endregion  

        #region Get User Details from Token
        private string GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType);
            return userIdClaim?.Value!;
        }

        private EmployeeType GetUserRoleFromToken()
        {
            return (EmployeeType)Enum.Parse(typeof(EmployeeType), User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value!);
        }
        private string GetUserNameFromToken()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType)?.Value!;
        }
        #endregion
    }
}
