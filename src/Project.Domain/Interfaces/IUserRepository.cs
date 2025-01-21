using Project.Domain.Entities;

namespace Project.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByFullName(string fullname, CancellationToken cancellationToken);
        Task<User> GetByUserName(string username, CancellationToken cancellationToken);
        Task<User> GetByEmail(string email, CancellationToken cancellationToken);
        Task<User> GetByConfirmationToken(string confirmationToken, CancellationToken cancellationToken);
        Task<User> GetByPasswordResetToken(string passwordToken, CancellationToken cancellationToken);
        Task<User> MarkAccountForDeletion(Guid userId, CancellationToken cancellationToken);
        Task<User> DeleteAccount(Guid userId, CancellationToken cancellationToken);
        Task<List<User>> GetUsersMarkedForDeletion(CancellationToken cancellationToken);
    }
}
