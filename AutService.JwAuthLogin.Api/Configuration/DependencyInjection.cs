using AutService.JwAuthLogin.Api.Middlewares;
using AutService.JwAuthLogin.Application.Configuration;
using AutService.JwAuthLogin.Infrastructure.Configurations;

namespace SegurosSura.Facturacion.Core.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void UseDependencyInjection(this IServiceCollection services)
        {
            var configuration = services
                .BuildServiceProvider()
                .GetService<IConfiguration>();

            services.AddApplicationDependencies();
            services.AddInfrastructureDependencies(configuration!);
            services.AddTransient<ExceptionHandlingMiddleware>();
        }
    }
}
