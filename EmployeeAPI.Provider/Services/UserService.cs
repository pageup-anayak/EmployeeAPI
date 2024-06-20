using EmployeeAPI.Contracts.enums;
using EmployeeAPI.Contracts.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EmployeeAPI.Provider.Services
{
    class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
        public string GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value!;
        }

        public string GetUserNameFromToken()
        {
            var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return userNameClaim?.Value!;
        }

        public EmployeeType GetUserRoleFromToken()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;
            return (EmployeeType)Enum.Parse(typeof(EmployeeType), roleClaim);
        }
    }
}
