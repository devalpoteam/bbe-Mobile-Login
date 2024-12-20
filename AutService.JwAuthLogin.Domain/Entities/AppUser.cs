using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AutService.JwAuthLogin.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } // Propiedad adicional
        public string GoogleId { get; set; } // ID de Google
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
