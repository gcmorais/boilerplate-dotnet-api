using MediatR;
using Project.Application.Models;

namespace Project.Application.UseCases.AuthUseCases.ResetPassword
{
    public class ResetPasswordRequest : IRequest<ResponseModel<string>>
    {
        public string Email { get; set; }
    }
}
