namespace AutService.JwAuthLogin.Domain.Models.Request
{
    public class ChangePassword
    {
        /// <summary>
        /// Contraseña actual del usuario.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Nueva contraseña del usuario.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
