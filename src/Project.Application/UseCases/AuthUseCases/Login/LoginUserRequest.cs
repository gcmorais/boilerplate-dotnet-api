using MediatR;

namespace Project.Application.UseCases.AuthUseCases.Login
{
    public class LoginUserRequest : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
