using Project.Domain.Interfaces;

namespace Project.Application.Services
{
    public class AccountDeletionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccountDeletionService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteMarkedAccountsAsync(CancellationToken cancellationToken)
        {
            var usersToDelete = await _userRepository.GetUsersMarkedForDeletion(cancellationToken);

            foreach (var user in usersToDelete)
            {
                if (user.CanBeDeleted())
                {
                    _userRepository.Delete(user);
                }
            }

            await _unitOfWork.Commit(cancellationToken);
        }
    }
}
