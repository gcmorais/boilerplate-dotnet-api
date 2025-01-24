using MediatR;
using Project.Application.UseCases.AuthUseCases.Login;

namespace Project.Application.UseCases.AuthUseCases.RefreshTokens
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public string RefreshToken { get; set; } // Token sent by client
    }
}
