using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AutService.JwAuthLogin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Obtiene una lista de todos los usuarios registrados.
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de usuarios con información básica, como su ID, nombre y roles asociados.
        /// </remarks>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet("list")]
        [SwaggerOperation(Summary = "Lista de todos los usuarios", Description = "Devuelve una lista de todos los usuarios registrados.")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUser();

            return Ok(users);
        }

        /// <summary>
        /// Actualiza la información de un usuario.
        /// </summary>
        /// <remarks>
        /// Permite actualizar el rol u otra información básica de un usuario.
        /// </remarks>
        /// <param name="model">Modelo que contiene la información del usuario a actualizar.</param>
        /// <returns>Un mensaje de éxito o error.</returns>
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Actualiza la información de un usuario", Description = "Permite actualizar la información de un usuario, como sus roles.")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel model)
        {
            var result = await _userRepository.UpdateUserRole(model);
            if (!result)
            {
                return BadRequest(new { message = "No se pudo actualizar el usuario." });
            }
            return Ok(new { message = "Usuario actualizado con éxito." });
        }
    }
}
