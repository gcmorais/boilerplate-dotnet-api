using MediatR;
using Project.Application.Models;

namespace Project.Application.UseCases.AuthUseCases.Enable2FA
{
    public class EnableTwoFactorRequest : IRequest<ResponseModel<string>>
    {
        public string Email { get; set; }
    }
}
