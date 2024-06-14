using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//Table Todo {
//  id integer [not null, pk]
//  title string [not null]
//  description string [not null]
//  Status Todo_Status [default: 'NotStarted']
//  createdAt timestamp [default: "now()"]
//  updatedAt timestamp
//  assignedTo integer [ref: - Employee.id]
//  assignedBy integer [ref: - Employee.id]
//}

namespace EmployeeAPI.Contracts.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required TodoStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required int AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public required Employee AssignedTo { get; set; }
        public required int AssignedById { get; set; }

        [ForeignKey(nameof(AssignedById))]
        public required Employee AssignedBy { get; set; }

        public List<Notification>? Notifications { get; set; }

    }
}
