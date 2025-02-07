using AutoMapper;
using MediatR;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AdminUseCases.BanUser
{
    public class BanUserHandler : IRequestHandler<BanUserRequest, BanUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BanUserHandler(IUserRepository userRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BanUserResponse> Handle(BanUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(request.UserId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.BanUser(request.BannedUntil);

            await _userRepository.Update(user);
            await _unitOfWork.Commit(cancellationToken);


            return new BanUserResponse
            {
                UserId = user.Id,
                IsBanned = user.IsUserBanned,
                BannedUntil = user.UserBannedUntil
            };
        }
    }
}
