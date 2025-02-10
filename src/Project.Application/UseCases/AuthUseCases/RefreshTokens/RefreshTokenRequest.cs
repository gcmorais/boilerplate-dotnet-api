using MediatR;
using Project.Application.UseCases.AuthUseCases.Login;

namespace Project.Application.UseCases.AuthUseCases.RefreshTokens
{
    public class RefreshTokenRequest : IRequest<LoginResponse>
    {
        public string Token { get; set; }
    }
}
