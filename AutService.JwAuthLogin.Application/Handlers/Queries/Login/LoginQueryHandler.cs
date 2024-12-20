using AutService.JwAuthLogin.Application.Contracts;
using AutService.JwAuthLogin.Domain.Entities;

namespace AutService.JwAuthLogin.Application.Handlers.Queries.Login;

public sealed class LoginQueryHandler(IDatabaseRepository<AppUser> users) : IRequestHandler<LoginQuery, LoginResponse>
{
    private readonly IDatabaseRepository<AppUser> _users = users;
    public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var logon = request.logon;
        var contratoEntity = _users.Find(x => x.Email == logon.Email).FirstOrDefault();
        if (contratoEntity == null)
        {

        }
     //   var response = new GetByIdContratoResponse(contratoEntity.ConvertToDTOByID());
        return new LoginResponse(true);
    }
}
