using AutService.JwAuthLogin.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutService.JwAuthLogin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        // 1. Crear un nuevo rol
        [HttpPost("create-role")]
        [Authorize(Roles = "Admin")] // Solo un usuario admin puede crear roles
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (await _userRepository.CreateRole(roleName))
                return BadRequest(new { message = "El rol ya existe." });
            else
                return Ok(new { message = $"Rol {roleName} creado con éxito." });
        }

        // 2. Asignar un rol a un usuario
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")] // Solo admins pueden asignar roles
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            return Ok(await _userRepository.AssignRole(email, roleName));
        }

        // 3. Listar roles de un usuario
        [HttpGet("user-roles")]
        [Authorize] // Cualquier usuario autenticado puede ver sus roles
        public async Task<IActionResult> GetUserRoles(string email)
        {
            return Ok(await _userRepository.GetUserRoles(email));
        }
    }
}