using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//enum Todo_Status {
//  NotStarted [note: "waiting to be started"]
//  InProgress
//  Completed
//  Approved
//  Rejected [note: "can only be rejected when its completed"]
//}
namespace EmployeeAPI.Contracts.enums
{
    public enum TodoStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Approved,
        Rejected
    }
}
