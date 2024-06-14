using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Table Notification {
//  id integer [not null, pk]
//  message string [not null]
//  isSeen bool [not null, default: false]
//  assignedTo integer [ref: - Employee.id]
//  assignedBy integer [ref: - Employee.id]
//  createdAt timestamp [default: "now()"]
//  updatedAt timestamp
//  todoId integer [ref: > Todo.id]
//}

namespace EmployeeAPI.Contracts.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public required bool IsSeen { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required int AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public required Employee AssignedTo { get; set; }
        public required int AssignedById { get; set; }

        [ForeignKey(nameof(AssignedById))]
        public required Employee AssignedBy { get; set; }
        public required int TodoId { get; set; }

        [ForeignKey(nameof(TodoId))]
        public required Todo Todo { get; set; }
    }
}
