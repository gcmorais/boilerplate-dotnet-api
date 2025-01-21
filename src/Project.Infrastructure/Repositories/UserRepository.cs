using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using Project.Infrastructure.Context;

namespace Project.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
        public async Task<List<User>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Users
                //.Include(u => u.Products) Answer including table relationship
                .ToListAsync(cancellationToken);
        }
        public async Task<User> GetByUserName(string username, CancellationToken cancellationToken)
        {
            return await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);
        }
        public async Task<User> GetByFullName(string fullname, CancellationToken cancellationToken)
        {
            return await _context.Users
            .FirstOrDefaultAsync(x => x.Fullname == fullname, cancellationToken);
        }
        public async Task<User> GetByConfirmationToken(string token, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token, cancellationToken);
        }
        public async Task<User> GetByPasswordResetToken(string token, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTimeOffset.UtcNow, cancellationToken);
        }
        public async Task<User> MarkAccountForDeletion(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }
        public async Task<User> DeleteAccount(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<List<User>> GetUsersMarkedForDeletion(CancellationToken cancellationToken)
        {
            return await _context.Users
                .Where(u => u.IsDeletionRequested && u.DeletionScheduledDate <= DateTimeOffset.UtcNow)
                .ToListAsync(cancellationToken);
        }
    }
}
