using Project.Domain.Entities;

namespace Project.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task RevokeAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
