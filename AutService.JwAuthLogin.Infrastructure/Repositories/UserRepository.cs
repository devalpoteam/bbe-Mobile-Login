using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Domain.Exceptions;
using AutService.JwAuthLogin.Domain.Models.Auth;
using AutService.JwAuthLogin.Domain.Models.Request;
using AutService.JwAuthLogin.Domain.Models.Response;

namespace AutService.JwAuthLogin.Infrastructure.Repositories
{
    public class UserRepository(IConfiguration configuration, 
                                UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager,
                                RoleManager<IdentityRole> roleManager) :IUserRepository
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
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
                var roles = await _userManager.GetRolesAsync(user); // Obtener los roles del usuario
                return GenerateUserToken(user, roles);
            }
            catch (InvalidOperationException ex)
            {
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
            var roles = await _userManager.GetRolesAsync(user); // Obtener los roles del usuario
            return GenerateUserToken(user, roles);
        }
        public async Task<bool> CreateRole(string roleName) {
            if (await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return true;
            return false;
        }
        public async Task<string> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return  "Usuario no encontrado." ;

            if (!await _roleManager.RoleExistsAsync(roleName))
                return "Rol no encontrado.";

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
                return $"Rol {roleName} asignado a {email}.";

            throw new AppException($"Error al registrar el usuario {email} al rol {roleName}");
        }
        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ["Usuario no encontrado."];

            var roles = await _userManager.GetRolesAsync(user);
            return [.. roles];
        }
        public async Task<List<UserModel>> GetAllUser()
        {
            return await _userManager.Users.Select(u => new UserModel
            {
                Email = u.Email,
                UserId = u.Id,
                UserName = u.UserName
            }).ToListAsync();
        }
        public async Task<bool> UpdateUserRole(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return false;// return NotFound(new { message = "Usuario no encontrado." });
            }

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return false;//( return BadRequest(new { message = "No se pudo actualizar el usuario." });
            }

            return true;// Ok(new { message = "Usuario actualizado con éxito." });
        }
        public async Task<bool> ChangePassword(ChangePassword model, ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
        public async Task<string> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // No revelamos si el email existe por razones de seguridad
                return null;
            }
            // Generar el token de restablecimiento
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return resetToken;
        }
        public async Task<bool> ResetPassword(string email,string Token, string NewPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, Token, NewPassword);

            if (!result.Succeeded)
            {
                return false;
            }
            return true;

        }
        //Eliminar usuario 
        public async Task<bool> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        //Desasignar rol
        public async Task<bool> RemoveRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return false;
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }
        #region Private Methods
        private UserToken GenerateUserToken(AppUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var secret = _configuration["Authentication:Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new AppException("JWT Secret is not configured.");
            }

            var key = Encoding.ASCII.GetBytes(secret);

            var expires = DateTime.UtcNow.AddDays(7);

            // Crear lista de claims base para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Authentication:Jwt:Subject"] ?? throw new AppException("JWT Subject is not configured.")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? throw new AppException("UserName is null.")),
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? throw new AppException("UserName is null.")),
                new Claim(ClaimTypes.Email, user.Email ?? throw new AppException("Email is null."))
            };

            // Agregar los roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crear el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
