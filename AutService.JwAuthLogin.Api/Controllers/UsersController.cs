using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Application.Services;
using AutService.JwAuthLogin.Domain.Models.Request;
using AutService.JwAuthLogin.Domain.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ResetPasswordRequest = AutService.JwAuthLogin.Domain.Models.Request.ResetPasswordRequest;

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

        ///<summary>
        ///Eliminar un usuario.
        ///</summary>
        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await _userRepository.DeleteUser(email);
            if (!result)
            {
                return BadRequest(new { message = "No se pudo eliminar el usuario." });
            }
            return Ok(new { message = "Usuario eliminado con éxito." });
        }

        /// <summary>
        /// Cambia la contraseña del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// El usuario debe estar autenticado y proporcionar su contraseña actual y una nueva contraseña.
        /// </remarks>
        /// <param name="model">Modelo que contiene las contraseñas actual y nueva.</param>
        /// <returns>Mensaje indicando el resultado del cambio.</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userRepository.ChangePassword(model,User);
            if (!result)
            {
                return BadRequest(new { message = "o se pudo cambiar la contraseña." });
            }
            return Ok(new { message = "Contraseña cambiada con éxito." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resetToken = await _userRepository.ForgotPassword(model.Email);
            if (resetToken == null)
            {
                // No revelamos si el email existe por razones de seguridad
                return Ok(new { message = "Si el correo está registrado, se enviará un email con instrucciones para restablecer la contraseña." });
            }           

            // Opcional: Enviar un enlace completo (si tienes una URL de frontend configurada)
            var resetUrl = $"https://frontend-url/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(model.Email)}";
            await EmailService.SendEmailAsync(
                      model.Email,
                      "Restablecimiento de contraseña",
                      $"Usa este enlace para restablecer tu contraseña: {resetUrl}",
                      $"<p>Usa este enlace para restablecer tu contraseña:</p><a href=\"{resetUrl}\">Restablecer contraseña</a>",
                      "FBT3HAJ2SD6FJVCN6SF58PGL"
                  );

            return Ok(new { message = "Si el correo está registrado, se enviará un email con instrucciones para restablecer la contraseña." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.ResetPassword(model.Email, model.Token,model.NewPassword);
            if (!user)
            {
                return BadRequest(new { message = "No se pudo restablecer la contraseña." });
            }

            return Ok(new { message = "Contraseña restablecida con éxito." });
        }
    }
}
