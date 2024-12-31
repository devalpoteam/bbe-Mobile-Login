using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Exceptions;
using AutService.JwAuthLogin.Domain.Models.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutService.JwAuthLogin.Api.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    public class AuthController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        [HttpGet("signin")]
        public Task SignInWithGoogle()
        {
            return HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleCallback")
            });
        }

        [HttpGet("callback")]
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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userRepository.CreateUserAsync(model.Email, model.Password, model.FullName);
                return Ok(new { Message = "Usuario creado exitosamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
        [HttpPost("login")]
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
                return Unauthorized(new {ex.Message });
            }
        }

    }
}