namespace AutService.JwAuthLogin.Api.Services
{
    public class TokenCleanupService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TokenRevocationService.CleanUpExpiredTokens();
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Limpia cada 10 minutos
            }
        }
    }
}
