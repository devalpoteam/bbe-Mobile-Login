using AutService.JwAuthLogin.Application.Behavior;

namespace AutService.JwAuthLogin.Application.Configuration
{
    public static class DependencyInjection
    {
        public static void AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
            });
        }
    }
}
