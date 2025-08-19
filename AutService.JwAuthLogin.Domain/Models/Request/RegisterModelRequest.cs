using System.ComponentModel.DataAnnotations;

namespace AutService.JwAuthLogin.Domain.Models.Request
{
    public class RegisterModelRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }
        public string Sexo { get; set; }
        public string Edad { get; set; }
    }
}
