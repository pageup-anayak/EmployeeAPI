using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contracts.Dtos.Requests.Departments
{
    public class CreateDepartmentRequest
    {
        public required string Name { get; set; }
    }
}
