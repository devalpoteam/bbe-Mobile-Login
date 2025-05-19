using AutService.JwAuthLogin.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AutService.JwAuthLogin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Crea un nuevo rol.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite a los administradores crear nuevos roles en el sistema.
        /// </remarks>
        /// <param name="roleName">El nombre del rol a crear.</param>
        /// <returns>Mensaje indicando el resultado de la operación.</returns>
        [HttpPost("create-role")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Crea un rol.", Description = "Este endpoint permite a los administradores crear roles.")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (await _userRepository.CreateRole(roleName))
                return BadRequest(new { message = "El rol ya existe." });
            else
                return Ok(new { message = $"Rol {roleName} creado con éxito." });
        }

        /// <summary>
        /// Asigna un rol a un usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite a los administradores asignar roles a usuarios existentes.
        /// </remarks>
        /// <param name="email">El email del usuario al que se asignará el rol.</param>
        /// <param name="roleName">El nombre del rol a asignar.</param>
        /// <returns>Mensaje indicando el resultado de la operación.</returns>
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Asigna un rol a un usuario.", Description = "Este endpoint permite a los administradores asignar roles a usuarios.")]
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            return Ok(await _userRepository.AssignRole(email, roleName));
        }

        /// <summary>
        /// Obtiene los roles de un usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite a cualquier usuario autenticado consultar los roles asociados a un usuario específico.
        /// </remarks>
        /// <param name="email">El email del usuario cuyos roles se desean consultar.</param>
        /// <returns>Lista de roles asociados al usuario.</returns>
        [HttpGet("user-roles")]
        [Authorize]
        [SwaggerOperation(Summary = "Consulta los roles de un usuario.", Description = "Devuelve una lista de roles asociados al usuario especificado.")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            return Ok(await _userRepository.GetUserRoles(email));
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve información básica del usuario actual, como su email, nombre de usuario y roles asociados.
        /// </remarks>
        /// <returns>Perfil del usuario autenticado.</returns>
        [HttpGet("profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene el perfil del usuario autenticado.", Description = "Devuelve información del usuario autenticado, como email, nombre y roles.")]
        public IActionResult GetUserProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var username = User.Identity.Name;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return Ok(new
            {
                Email = email,
                Username = username,
                Roles = roles
            });
        }
    }
}
