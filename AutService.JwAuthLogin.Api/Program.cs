using AutService.JwAuthLogin.Api.Extensions;
using AutService.JwAuthLogin.Api.Middlewares;
using AutService.JwAuthLogin.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SegurosSura.Facturacion.Core.Api.Configuration;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.ConfigureAppSwagger();
    builder.Services.UseDependencyInjection();
    builder.Services.ConfigureAppSqlDatabase(builder.Configuration);
    builder.Services.AddAppIdentity(builder.Configuration);
    builder.Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());

    builder.Services.AddMvc(options => options.RespectBrowserAcceptHeader = true);
  
    var app = builder.Build();

    app.MapSwagger();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GOOGLE AUTH SAMPLE API V1");
        options.RoutePrefix = string.Empty;
    });
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseCors(it => it.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        await scopeFactory.EnsureDatabaseMigratedAsync();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception on starting ap: Error: {ex}.");
}


