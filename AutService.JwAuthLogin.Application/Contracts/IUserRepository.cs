using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Domain.Models.Auth;
using AutService.JwAuthLogin.Domain.Models.Request;
using AutService.JwAuthLogin.Domain.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AutService.JwAuthLogin.Application.Contracts
{
    public interface IUserRepository
    {
        Task<UserToken> GetOrCreateExternalLoginUser(string key, string email, string fullname);
        Task<IdentityResult> CreateUserAsync(string email, string password, string fullname);
        Task<UserToken> AuthenticateUserAsync(string email, string password);
        Task<bool> CreateRole(string roleName);
        Task<string> AssignRole(string email, string roleName);
        Task<List<string>> GetUserRoles(string email);
        Task<List<UserModel>> GetAllUser();
        Task<bool> UpdateUserRole(UserModel user);
        Task<bool> ChangePassword(ChangePassword model, ClaimsPrincipal user);
        Task<string> ForgotPassword(string email);
        Task<bool> ResetPassword(string email, string Token, string NewPassword);
        Task<bool> RemoveRole(string email, string roleName);
        Task<bool> DeleteUser(string email);


    }
}
