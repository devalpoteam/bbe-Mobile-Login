namespace AutService.JwAuthLogin.Domain.Models.Response
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Sexo { get; set; }
        public string Edad { get; set; }

        public bool premium { get; set; }
        public DateTime ultimoPago { get; set; }

    }
}
