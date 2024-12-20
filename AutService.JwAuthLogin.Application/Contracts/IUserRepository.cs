using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Domain.Models.Auth;

namespace AutService.JwAuthLogin.Application.Contracts
{
    public interface IUserRepository
    {
        Task<UserToken> GetOrCreateExternalLoginUser(string key, string email, string fullname);
        Task<IdentityResult> CreateUserAsync(string email, string password, string fullname);
        Task<UserToken> AuthenticateUserAsync(string email, string password);
    }
}
