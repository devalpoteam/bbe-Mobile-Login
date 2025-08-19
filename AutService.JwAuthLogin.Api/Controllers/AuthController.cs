using AutService.JwAuthLogin.Api.Services;
using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Exceptions;
using AutService.JwAuthLogin.Domain.Models.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace AutService.JwAuthLogin.Api.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    public class AuthController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Redirige al usuario para autenticarse mediante Google.
        /// </summary>
        /// <remarks>
        /// Este endpoint inicia el flujo de autenticación con Google. Redirige al usuario a la página de login de Google.
        /// </remarks>
        [HttpGet("signin")]
        [SwaggerOperation(Summary = "Inicia sesión con Google", Description = "Redirige al usuario a Google para autenticación.")]
        public Task SignInWithGoogle()
        {
            return HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleCallback")
            });
        }

        /// <summary>
        /// Procesa el callback de Google después de la autenticación.
        /// </summary>
        /// <remarks>
        /// Este endpoint recibe la respuesta de Google después de que el usuario se autentica y genera un token JWT para el usuario.
        /// </remarks>
        /// <returns>Un token JWT con la información del usuario.</returns>
        [HttpGet("callback")]
        [SwaggerOperation(Summary = "Procesa el callback de Google", Description = "Recibe la información del usuario autenticado desde Google y genera un token JWT.")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            // Extraer información del usuario autenticado
            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var googleId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("No se pudo obtener el email del usuario.");
            }

            var token = await _userRepository.GetOrCreateExternalLoginUser(googleId, email, name);

            return Ok(new
            {
                Message = "Autenticación exitosa",
                token.Token,
                token.Expires
            });
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="model">Modelo que contiene el email, contraseña y nombre completo.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registra un nuevo usuario", Description = "Permite registrar un usuario en el sistema con email y contraseña.")]
        public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userRepository.CreateUserAsync(model.Email, model.Password, model.FullName,model.Sexo, model.Edad );
                return Ok(new { Message = "Usuario creado exitosamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Autentica a un usuario con email y contraseña.
        /// </summary>
        /// <param name="model">Modelo que contiene el email y contraseña del usuario.</param>
        /// <returns>Token JWT y fecha de expiración si la autenticación es exitosa.</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Inicia sesión con email y contraseña", Description = "Permite autenticar un usuario con sus credenciales y devuelve un token JWT.")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var token = await _userRepository.AuthenticateUserAsync(model.Email, model.Password);
                return Ok(new
                {
                    Message = "Autenticación exitosa.",
                    token.Token,
                    token.Expires
                });
            }
            catch (AppException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }
        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        /// <remarks>
        /// Este endpoint simplemente elimina cualquier token activo almacenado en memoria o base de datos si se usa revocación de tokens.
        /// </remarks>
        /// <returns>Mensaje indicando el resultado de la operación.</returns>
        [HttpPost("logout")]
        [Authorize] // Solo usuarios autenticados pueden cerrar sesión
        public IActionResult Logout()
        {
            var token = Request.Headers.Authorization.ToString()?.Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                // Revocar el token actual
                TokenRevocationService.RevokeToken(token, DateTime.UtcNow.AddHours(1)); // Asume una duración de 1 hora para el token
            }

            return Ok(new { message = "Sesión cerrada con éxito." });
        }
    }
}
