using EmployeeAPI.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUserService _userService;
        public TodoController(IEmployeeService employeeService,
                              IDepartmentService departmentService,
                              ITodoService todoService,
                              ILogger<TodoController> logger,
                              IUserService userService)
        {
            _todoService = todoService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _logger = logger;
            _userService = userService;
        }

        #region Get Todo
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        public async Task<IActionResult> GetTodo(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching todo with ID: {id}");
                if (!await _todoService.TodoExists(id))
                {
                    _logger.LogWarning($"Todo with ID {id} does not exist");
                    return BadRequest("Todo dose not exists");
                }
                var todo = await _todoService.GetTodoByIdAsync(id);
                _logger.LogInformation($"Fetched todo with ID: {id}");
                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching todo with ID {id}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching todo");
            }
        }
        #endregion
        //TODO: update below method to have TodoStatus as well
        #region Get Todos 
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Todo>))]
        public async Task<IActionResult> GetTodos()
        {
            try
            {
                // extract out the user details from the token
                // id, role, name

                var empId = _userService.GetUserIdFromToken();
                EmployeeType role = _userService.GetUserRoleFromToken();
                _logger.LogInformation($"Fetching todos for Employee ID: {empId}, Role: {role}");
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
                _logger.LogInformation($"Fetched {todos.Count} todos for Employee ID: {empId}");
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching todos: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching todos");
            }
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
            try
            {
                _logger.LogInformation($"Fetching todos for Employee ID: {employeeId}");
                var employee = await _employeeService.GetEmployee(employeeId, null);
                if (employee == null)
                {
                    _logger.LogWarning($"Employee with ID {employeeId} does not exist");
                    return BadRequest("Employee dose not exists");
                }
                var role = _userService.GetUserRoleFromToken();
                var employeeRole = employee.EmployeeType;
                if (role == EmployeeType.Admin)
                {
                    // Logic for Admin role
                    Console.WriteLine("Role is Admin");

                    if (employeeRole == EmployeeType.SuperAdmin)
                    {
                        // Logic when employeeRole is SuperAdmin
                        _logger.LogWarning("Admin cannot access SuperAdmin data");
                        return BadRequest("Admin can access SuperAdmin data");
                    }
                    else if (employeeRole == EmployeeType.Admin)
                    {
                        _logger.LogWarning("Admin cannot access other admin data");
                        // Logic when employeeRole is Admin
                        return BadRequest("Admin can access other admin data");
                    }
                    else
                    {
                        // Logic when employeeRole is Employee
                        int adminId = int.Parse(_userService.GetUserIdFromToken());

                        var admin = (await _employeeService.GetEmployee(adminId, null))!;
                        if (admin.DepartmentId != employee.DepartmentId)
                        {
                            _logger.LogWarning("Admin cannot access other department employees' tasks");
                            return BadRequest("Admin can not access other department emloyees task");
                        }
                        else
                        {
                            var todos = await _todoService.GetTodosByEmployeeIdAsync(employeeId);
                            _logger.LogInformation($"Fetched {todos.Count} todos for Employee ID: {employeeId}");
                            return Ok(todos);
                        }
                    }
                }
                else
                {
                    // Logic for SuperAdmin role
                    _logger.LogInformation("Role is SuperAdmin");

                    if (employeeRole == EmployeeType.SuperAdmin)
                    {
                        _logger.LogWarning("SuperAdmin cannot access another SuperAdmin's data");

                        // Logic when employeeRole is SuperAdmin
                        return BadRequest("SuperAdmin can not access the SuperAdmin");
                    }
                    else if (employeeRole == EmployeeType.Admin)
                    {
                        // Logic when employeeRole is Admin
                        var todos = await _todoService.GetTodosByEmployeeIdAsync(employeeId);
                        _logger.LogInformation($"Fetched {todos.Count} todos for Employee ID: {employeeId}");
                        return Ok(todos);
                    }
                    else
                    {
                        // Logic when employeeRole is Employee
                        var todos = await _todoService.GetTodosByEmployeeIdAsync(employeeId);
                        _logger.LogInformation($"Fetched {todos.Count} todos for Employee ID: {employeeId}");
                        return Ok(todos);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching todos for Employee ID {employeeId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching todos");
            }
        }
        #endregion

        #region Update Todo
        [HttpPost("{id}")]
        [ProducesResponseType(200, Type = typeof(List<string>))]
        //[Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest todoUpdateDto)
        {
            try
            {
                _logger.LogInformation($"Updating todo with ID: {id}");
                Todo? todo = await _todoService.GetTodoByIdAsync(id);

                if (todo == null)
                {
                    _logger.LogWarning($"Todo with ID {id} does not exist");
                    return BadRequest("Todo does not exist");
                }

                var role = _userService.GetUserRoleFromToken();
                List<string> res = new List<string>();

                switch (role)
                {
                    case EmployeeType.Admin:
                        _logger.LogInformation("Role is Admin");

                        if (todoUpdateDto.Title != null)
                        {
                            todo.Title = todoUpdateDto.Title;
                            res.Add("Updated title");
                            _logger.LogInformation("Updated title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            todo.Description = todoUpdateDto.Description;
                            res.Add("Updated description");
                            _logger.LogInformation("Updated description");
                        }

                        if (todoUpdateDto.Status != null)
                        {
                            if ((todoUpdateDto.Status.Value == TodoStatus.Approved || todoUpdateDto.Status.Value == TodoStatus.Rejected) && todo.Status == TodoStatus.Completed)
                            {
                                todo.Status = todoUpdateDto.Status.Value;
                                res.Add("Updated status");
                                _logger.LogInformation("Updated status");
                            }
                            else
                            {
                                _logger.LogWarning($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }
                        }

                        if (todoUpdateDto.AssignedToId != null)
                        {
                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedToId, null);
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedToId, null))
                            {
                                _logger.LogWarning($"Employee with ID {todoUpdateDto.AssignedToId} does not exist");
                                return BadRequest("Employee does not exist");
                            }

                            if (employee.EmployeeType != EmployeeType.Employee)
                            {
                                _logger.LogWarning($"Cannot assign task to {employee.EmployeeType}");
                                return BadRequest($"Cannot assign task to {employee.EmployeeType}");
                            }

                            todo.AssignedToId = (int)todoUpdateDto.AssignedToId;
                            res.Add("Updated AssignedTo");
                            _logger.LogInformation("Updated AssignedTo");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            _logger.LogWarning("Admin cannot change AssignedTo");
                            return BadRequest("Admin cannot change AssignedTo");
                        }
                        break;

                    case EmployeeType.SuperAdmin:
                        _logger.LogInformation("Role is SuperAdmin");

                        if (todoUpdateDto.Title != null)
                        {
                            todo.Title = todoUpdateDto.Title;
                            res.Add("Updated title");
                            _logger.LogInformation("Updated title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            todo.Description = todoUpdateDto.Description;
                            res.Add("Updated description");
                            _logger.LogInformation("Updated description");
                        }

                        if (todoUpdateDto.Status != null)
                        {
                            if ((todoUpdateDto.Status.Value == TodoStatus.Approved || todoUpdateDto.Status.Value == TodoStatus.Rejected) && todo.Status == TodoStatus.Completed)
                            {
                                todo.Status = todoUpdateDto.Status.Value;
                                res.Add("Updated status");
                                _logger.LogInformation("Updated status");
                            }
                            else
                            {
                                _logger.LogWarning($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }
                        }

                        if (todoUpdateDto.AssignedToId != null)
                        {
                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedToId, null);
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedToId, null))
                            {
                                _logger.LogWarning($"Employee with ID {todoUpdateDto.AssignedToId} does not exist");
                                return BadRequest("Employee does not exist");
                            }

                            if (employee.EmployeeType != EmployeeType.Employee)
                            {
                                _logger.LogWarning($"Cannot assign task to {employee.EmployeeType}");
                                return BadRequest($"Cannot assign task to {employee.EmployeeType}");
                            }

                            todo.AssignedToId = (int)todoUpdateDto.AssignedToId;
                            res.Add("Updated AssignedTo");
                            _logger.LogInformation("Updated AssignedTo");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            if (!await _employeeService.EmployeeExists(todoUpdateDto.AssignedById, null))
                            {
                                _logger.LogWarning($"Employee with ID {todoUpdateDto.AssignedById} does not exist");
                                return BadRequest("Employee does not exist");
                            }

                            var employee = await _employeeService.GetEmployee(todoUpdateDto.AssignedById, null);
                            if (employee.EmployeeType != EmployeeType.Admin)
                            {
                                _logger.LogWarning($"Cannot assign task to {employee.EmployeeType}");
                                return BadRequest($"Cannot assign task to {employee.EmployeeType}");
                            }

                            todo.AssignedById = (int)todoUpdateDto.AssignedById;
                            res.Add("Updated AssignedBy");
                            _logger.LogInformation("Updated AssignedBy");
                        }
                        break;

                    default:
                        _logger.LogInformation("Role is Employee");

                        if (todoUpdateDto.Status != null)
                        {
                            if (todoUpdateDto.Status == TodoStatus.Approved)
                            {
                                _logger.LogWarning($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                                return BadRequest($"{todo.Status} can not be converted to ${todoUpdateDto.Status}");
                            }

                            if (TodoStatusExtensions.CanTransitionTo(todo.Status, (TodoStatus)todoUpdateDto.Status))
                            {
                                todo.Status = (TodoStatus)todoUpdateDto.Status;
                                res.Add("Updated status");
                                _logger.LogInformation("Updated status");
                            }
                        }

                        if (todoUpdateDto.Title != null)
                        {
                            _logger.LogWarning("Employee cannot update title");
                            return BadRequest("Employee cannot update title");
                        }

                        if (todoUpdateDto.Description != null)
                        {
                            _logger.LogWarning("Employee cannot update description");
                            return BadRequest("Employee cannot update description");
                        }

                        if (todoUpdateDto.AssignedToId != null)
                        {
                            _logger.LogWarning("Employee cannot assign task");
                            return BadRequest("Employee cannot assign task");
                        }

                        if (todoUpdateDto.AssignedById != null)
                        {
                            _logger.LogWarning("Employee cannot assign task");
                            return BadRequest("Employee cannot assign task");
                        }
                        break;
                }

                if (res.Count == 0)
                {
                    res.Add("No updates applied");
                    _logger.LogInformation("No updates applied");
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating todo with ID {id}: {ex.Message}");
                return StatusCode(500, "An error occurred while updating todo");
            }
        }
        #endregion

        #region Create Todo
        [HttpPost]
        public async Task<IActionResult> CreateTodoAsync([FromBody] CreateTodoRequest createTodo)
        {
            try
            {
                _logger.LogInformation("Creating new todo");
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for createTodo request");
                    return BadRequest(ModelState);
                }
                var (title, description, assignedToId) = createTodo;

                var emp1 = await _employeeService.GetEmployee(assignedToId, null);
                if (emp1.EmployeeType != EmployeeType.Employee)
                {
                    _logger.LogWarning($"Cannot assign to {emp1.EmployeeType}");
                    return BadRequest($"Can assign to {emp1.EmployeeType}");
                }
                return Ok(await _todoService.AddTodoAsync(createTodo));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating todo: {ex.Message}");
                return StatusCode(500, "An error occured while creating todo");
            }
        }
        #endregion
    }
}
