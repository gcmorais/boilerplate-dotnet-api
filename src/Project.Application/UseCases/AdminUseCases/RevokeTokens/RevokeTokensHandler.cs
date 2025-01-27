using MediatR;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AdminUseCases.RevokeTokens
{
    public class RevokeTokensHandler : IRequestHandler<RevokeTokensCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RevokeTokensHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Unit> Handle(RevokeTokensCommand request, CancellationToken cancellationToken)
        {
            var tokens = await _refreshTokenRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (tokens == null || !tokens.Any())
            {
                throw new KeyNotFoundException("No tokens found for the specified user.");
            }

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
