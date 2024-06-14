using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using EmployeeAPI.Contracts.enums;

//Table Employee {
//  id integer unique [not null, primary key]
//  name string [not null]
//  employeeType Employee_Type [not null] 
//  email string  [unique, not null]
//  address string
//  city string 
//  country string
//  createdAt timestamp [default: `now()`]
//  updatedAt timestamp
//  password string [note: "password stored after encription"]
//  departmentId integer [ref: < Department.id]
//}
namespace EmployeeAPI.Contracts.Models
{
    public class Employee
    {
        [NotNull]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required EmployeeType EmployeeType { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public required Department Department { get; set; }
        public List<Todo>? CreatedTodos { get; set; }
        public List<Todo>? AssignedTodos { get; set; }
        public List<Notification>? CreatedNotifications { get; set; }
        public List<Notification>? GotNotifications { get; set; }
    }
}
