using AutService.JwAuthLogin.Domain.ValueObject;
using System;
namespace AutService.JwAuthLogin.Infrastructure.Context
{
    public static class SetupPersistence
    {
        public static void ConfigureAppSqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseSetting = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            services.AddDbContext<DatabaseContext>(it =>
            
                it.UseSqlServer(databaseSetting!.ConnectionString), ServiceLifetime.Scoped); 
           // );
        }
        public static async Task EnsureDatabaseMigratedAsync(this IServiceScopeFactory scopeFactory)
        {
            try
            {
                await using var serviceScope = scopeFactory.CreateAsyncScope();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    await dbContext.Database.MigrateAsync();
                }
                else
                {
                    Console.WriteLine("Database is up to date.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error ensuring the database is migrated", e);
            }
        }
    }
}
