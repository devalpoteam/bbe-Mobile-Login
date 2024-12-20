namespace AutService.JwAuthLogin.Domain.Exceptions.Base
{
    public abstract class ConflictException(string message) : Exception(message)
    {
    }
}
