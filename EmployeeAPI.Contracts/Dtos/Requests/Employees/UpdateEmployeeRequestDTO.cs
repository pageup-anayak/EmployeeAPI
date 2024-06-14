﻿using EmployeeAPI.Contracts.enums;

namespace EmployeeAPI.Contracts.Dtos.Requests.Employees
{
    public class UpdateEmployeeRequestDTO
    {
        public required string Name { get; set; }
        public required EmployeeType EmployeeType { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
