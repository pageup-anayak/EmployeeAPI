using EmployeeAPI.Contracts.enums;
namespace EmployeeAPI.Contracts.Interfaces
{
    public interface IUserService
    {
        string GetUserIdFromToken();
        EmployeeType GetUserRoleFromToken();
        string GetUserNameFromToken();
    }
}
