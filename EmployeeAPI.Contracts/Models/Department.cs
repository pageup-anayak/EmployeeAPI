using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Table Department {
//  id integer [not null, unique, pk]
//  name string [not null, unique]
//  createdAt timestamp [default: "now()"]
//  updatedAt timestamp
//}

namespace EmployeeAPI.Contracts.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Employee>? Employees { get; set; }

        public Department()
        {
            Employees = new List<Employee>();
        }
    }
}
