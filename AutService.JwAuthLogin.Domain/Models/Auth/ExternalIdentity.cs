using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AutService.JwAuthLogin.Domain.Models.Auth
{
    public class GoogleUserRequest
    {
        public const string PROVIDER = "google";

        [JsonProperty("idToken")]
        [Required]
        public string IdToken { get; set; }

    }
}