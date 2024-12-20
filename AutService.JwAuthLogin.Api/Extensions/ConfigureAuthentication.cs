using AutService.JwAuthLogin.Domain.Entities;
using AutService.JwAuthLogin.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;

namespace AutService.JwAuthLogin.Api.Extensions
{
    public static class ConfigureAuthentication
    {
        public static void AddAppIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DatabaseContext>()
              .AddDefaultTokenProviders();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                var jwtSettings = configuration.GetSection("Authentication:Jwt");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true
                };
            })
             .AddGoogleOpenIdConnect(options =>
             {
                 options.ClientId = configuration["Authentication:Google:ClientId"];
                 options.ClientSecret = configuration["Authentication:Google:ClientSecret"];

             }).AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
             {
                 googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                 googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                 googleOptions.CallbackPath = "/api/googleauth/callback";
                 googleOptions.SaveTokens = true;

                 // Deshabilitar validación de correlación
                 googleOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                 googleOptions.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;

                 googleOptions.Events = new OAuthEvents
                 {
                     OnRemoteFailure = context =>
                     {
                         // Log del error
                         Debug.WriteLine($"Error en OAuth: {context.Failure?.Message}");
                         return Task.CompletedTask;
                     }
                 };
             }); ;
        }
    }
}
