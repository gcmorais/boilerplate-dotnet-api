using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Infrastructure.Context;

namespace Project.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task RevokeAsync(Guid id, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);

            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
