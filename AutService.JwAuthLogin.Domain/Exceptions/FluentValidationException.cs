using AutService.JwAuthLogin.Domain.Exceptions.Base;

namespace AutService.JwAuthLogin.Domain.Exceptions
{
    public sealed class FluentValidationException(Dictionary<string, string[]> errors)
        : BadRequestException("Se produjeron errores de validación")
    {
        public Dictionary<string, string[]> Errors { get; } = errors;
    }
}
