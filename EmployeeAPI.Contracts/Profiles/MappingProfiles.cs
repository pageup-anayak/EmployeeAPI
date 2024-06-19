using AutoMapper;
using EmployeeAPI.Contracts.Dtos.Requests.Employees;
using EmployeeAPI.Contracts.Dtos.Requests.Todos;
using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contracts.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Employee, CreateEmployeeRequestDTO>().ReverseMap();
            CreateMap<Employee, UpdateEmployeeRequestDTO>().ReverseMap();

            CreateMap<Todo, CreateTodoRequest>().ReverseMap();
            CreateMap<Todo, UpdateTodoRequest>().ReverseMap();
        }
    }
}
