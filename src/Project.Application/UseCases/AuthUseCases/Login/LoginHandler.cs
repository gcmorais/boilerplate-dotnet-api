using MediatR;
using Project.Application.Interfaces;
using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.Login
{
    public class LoginHandler : IRequestHandler<LoginUserRequest, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ICreateVerifyHash _serviceHash;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginHandler(IUserRepository userRepository, ITokenService tokenService, ICreateVerifyHash serviceHash, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _serviceHash = serviceHash;
            _emailService = emailService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null || !_serviceHash.PasswordVerify(request.Password, user.HashPassword, user.SaltPassword))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (user.IsTwoFactorEnabled)
            {
                user.GenerateTwoFactorCode();
                await _userRepository.Update(user);

                await _emailService.SendTwoFactorCode(user.Email, user.TwoFactorCode);

                return new LoginResponse { RequiresTwoFactor = true };
            }

            // Generate tokens
            var token = _tokenService.GenerateToken(user, user.Roles);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
            };

            // Save the Refresh Token in the db
            await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token
            };
        }

    }
}
