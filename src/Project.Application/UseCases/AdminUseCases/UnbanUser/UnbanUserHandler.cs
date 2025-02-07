using MediatR;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AdminUseCases.UnbanUser
{
    public class UnbanUserHandler : IRequestHandler<UnbanUserRequest, UnbanUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnbanUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<UnbanUserResponse> Handle(UnbanUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(request.UserId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.UnbanUser();

            await _userRepository.Update(user);
            await _unitOfWork.Commit(cancellationToken);

            return new UnbanUserResponse
            {
                UserId = user.Id,
                IsBanned = user.IsUserBanned,
                BannedUntil = user.UserBannedUntil,
                Description = "Successfully unbanned!!"
            };
        }
    }
}
