using Project.Domain.Entities;

namespace Project.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task RevokeAsync(Guid id, CancellationToken cancellationToken);
    }
}
