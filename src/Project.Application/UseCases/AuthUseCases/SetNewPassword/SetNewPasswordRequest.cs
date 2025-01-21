using MediatR;
using Project.Application.Models;

namespace Project.Application.UseCases.AuthUseCases.SetNewPassword
{
    public class SetNewPasswordRequest : IRequest<ResponseModel<string>>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
