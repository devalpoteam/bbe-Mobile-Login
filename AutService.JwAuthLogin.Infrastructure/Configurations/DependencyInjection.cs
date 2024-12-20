
using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Infrastructure.Repositories;

namespace AutService.JwAuthLogin.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddScoped(typeof(IDatabaseRepository<>), typeof(DatabaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();

        }
    }
}
