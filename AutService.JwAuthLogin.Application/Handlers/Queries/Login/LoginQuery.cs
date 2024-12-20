using AutService.JwAuthLogin.Domain.Models.Request;

namespace AutService.JwAuthLogin.Application.Handlers.Queries.Login;

public record LoginQuery(LoginRequest logon) : IRequest<LoginResponse>;
