using FpaManagement.Application.Common.Models;

namespace FpaManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result<string> Result, string UserId)> CreateUserAsync(string userName, string email, string password, IEnumerable<string> roles);
    Task<Result<string>> DeleteUserAsync(string userId);
   
}
