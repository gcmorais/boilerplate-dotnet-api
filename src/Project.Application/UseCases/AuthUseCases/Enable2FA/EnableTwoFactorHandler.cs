using MediatR;
using Project.Application.Models;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.Enable2FA
{
    public class EnableTwoFactorHandler : IRequestHandler<EnableTwoFactorRequest, ResponseModel<string>>
    {
        private readonly IUserRepository _userRepository;

        public EnableTwoFactorHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseModel<string>> Handle(EnableTwoFactorRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.EnableTwoFactorAuthentication();

            await _userRepository.Update(user);

            return new ResponseModel<string>("Two-factor authentication enabled.", true);
        }
    }
}
