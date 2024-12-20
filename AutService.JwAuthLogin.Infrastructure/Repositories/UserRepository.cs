using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Domain.Exceptions;
using AutService.JwAuthLogin.Domain.Models.Auth;

namespace AutService.JwAuthLogin.Infrastructure.Repositories
{
    public class UserRepository(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager):IUserRepository
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UserToken> GetOrCreateExternalLoginUser(string key, string email, string fullname)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Email = email,
                        UserName = email,
                        Id = key,
                        FullName = fullname
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new AppException("Error al registrar el usuario");
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return GenerateUserToken(user);
            }
            catch (InvalidOperationException ex)
            {
                // Registrar los detalles de la excepción
                // Manejar el caso específico de la conexión cerrada
                throw new AppException("Problema de conexión a la base de datos", ex);
            }
            catch (Exception e) {
                return null;
            }
        }
        public async Task<IdentityResult> CreateUserAsync(string email, string password, string fullname)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new AppException("El usuario ya existe con ese email.");
            }

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullname
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new AppException("Error al crear el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }
        public async Task<UserToken> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
            {
                throw new AppException("Email o contraseña incorrectos.");
            }

            return GenerateUserToken(user);
        }
        #region Private Methods
        private UserToken GenerateUserToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var secret = _configuration["Authentication:Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new AppException("JWT Secret is not configured.");
            }

            var key = Encoding.ASCII.GetBytes(secret);

            var expires = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.Name, user.Id),
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Authentication:Jwt:Subject"] ?? throw new AppException("JWT Subject is not configured.")),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Name, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName ?? throw new AppException("UserName is null.")),
                        new Claim(ClaimTypes.NameIdentifier, user.UserName ?? throw new AppException("UserName is null.")),
                        new Claim(ClaimTypes.Email, user.Email ?? throw new AppException("Email is null."))
                    }),

                Expires = expires,

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Authentication:Jwt:Issuer"],
                Audience = _configuration["Authentication:Jwt:Audience"]
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(securityToken);

            return new UserToken
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                Expires = expires
            };
        }
        #endregion
    }
}
