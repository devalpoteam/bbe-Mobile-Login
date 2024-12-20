namespace AutService.JwAuthLogin.Domain.ValueObject
{
    public class DatabaseSettings(string connectionString)
    {
        public string ConnectionString { get; set; } = connectionString;
    }
}
