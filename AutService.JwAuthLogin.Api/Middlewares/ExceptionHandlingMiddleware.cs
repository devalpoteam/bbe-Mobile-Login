using AutService.JwAuthLogin.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AutService.JwAuthLogin.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private const string JsonContentType = "application/json";

        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, message: e.Message);

                await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = JsonContentType;

            var exceptionType = exception.GetType();

            httpContext.Response.StatusCode = exception switch
            {
                var _ when exceptionType == typeof(UnauthorizedAccessException) => StatusCodes.Status401Unauthorized,
                var _ when exceptionType == typeof(ValidationException) => StatusCodes.Status422UnprocessableEntity,
                AppException e when exceptionType == typeof(AppException) => e.Code,
                _ => StatusCodes.Status500InternalServerError,
            };
            var response = new ErrorResponse
            {
                Status = httpContext.Response.StatusCode,
                Message = exception.Message
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}