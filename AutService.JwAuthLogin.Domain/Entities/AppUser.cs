using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AutService.JwAuthLogin.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } // Propiedad adicional
        public string Sexo { get; set; } // Propiedad adicional
        public string Edad { get; set; } // Propiedad adicional
        
        public bool premium { get; set; } = false; // Propiedad adicional

        public DateTime ultimoPago { get; set; }
        public string GoogleId { get; set; } // ID de Google
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
