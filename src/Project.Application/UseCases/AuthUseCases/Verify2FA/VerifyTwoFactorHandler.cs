using MediatR;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.Login;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.Verify2FA
{
    public class VerifyTwoFactorHandler : IRequestHandler<VerifyTwoFactorRequest, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public VerifyTwoFactorHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(VerifyTwoFactorRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null || !user.ValidateTwoFactorCode(request.Code))
            {
                throw new UnauthorizedAccessException("Invalid two-factor code.");
            }

            var token = _tokenService.GenerateToken(user, user.Roles);

            return new LoginResponse { Token = token };
        }
    }
}
