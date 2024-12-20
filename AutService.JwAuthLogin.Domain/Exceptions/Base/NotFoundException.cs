namespace AutService.JwAuthLogin.Domain.Exceptions.Base
{
    public abstract class NotFoundException(string message) : Exception(message)
    {
    }
}
