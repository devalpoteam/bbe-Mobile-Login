using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Domain.ValueObject;

namespace AutService.JwAuthLogin.Infrastructure.Context
{
    public static class SetupPersistence
    {
        public static void ConfigureAppSqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseSetting = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            services.AddDbContext<DatabaseContext>(it =>
                it.UseNpgsql(databaseSetting!.ConnectionString) // 💡 Cambiado a PostgreSQL
                , ServiceLifetime.Scoped
            );
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

        public static async Task EnsureCreatedUserAdminAsync(this IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            try
            {
                await using var serviceScope = scopeFactory.CreateAsyncScope();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roleConfiguration = configuration.GetSection(nameof(RoleConfiguration)).Get<RoleConfiguration>();

                foreach (var userConfig in roleConfiguration.Users)
                {
                    // Crear roles si no existen
                    foreach (var role in userConfig.Roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    // Crear usuario si no existe
                    var user = await userManager.FindByEmailAsync(userConfig.Email);
                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = userConfig.Email,
                            Email = userConfig.Email,
                            FullName = "Usuario asignado automáticamente"
                        };

                        var result = await userManager.CreateAsync(user, userConfig.Password);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Error al crear el usuario {userConfig.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }

                    // Asignar roles al usuario
                    foreach (var role in userConfig.Roles)
                    {
                        if (!await userManager.IsInRoleAsync(user, role))
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error ensuring the database is migrated", e);
            }
        }
    }
}