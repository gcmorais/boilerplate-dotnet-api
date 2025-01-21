using MediatR;
using Project.Application.UseCases.AuthUseCases.Login;

namespace Project.Application.UseCases.AuthUseCases.Verify2FA
{
    public class VerifyTwoFactorRequest : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
