using MediatR;
using Project.Application.Interfaces;
using Project.Application.UseCases.AuthUseCases.Login;
using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.RefreshTokens
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, LoginResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Token, cancellationToken);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _userRepository.GetById(refreshToken.UserId, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            // Generate new tokens
            var newJwtToken = _tokenService.GenerateToken(user, user.Roles);
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
            };

            // Revoke the old and save the new
            await _refreshTokenRepository.RevokeAsync(refreshToken.Id, cancellationToken);
            await _refreshTokenRepository.CreateAsync(newRefreshToken, cancellationToken);

            return new LoginResponse
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
