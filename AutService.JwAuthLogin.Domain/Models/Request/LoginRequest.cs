using System.ComponentModel.DataAnnotations;

namespace AutService.JwAuthLogin.Domain.Models.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
