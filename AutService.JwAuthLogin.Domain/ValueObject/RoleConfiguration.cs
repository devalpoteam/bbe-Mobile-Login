namespace AutService.JwAuthLogin.Domain.ValueObject
{
    public class RoleConfiguration
    {
        public List<UserRoleConfig> Users { get; set; } = [];
    }
    public class UserRoleConfig
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
