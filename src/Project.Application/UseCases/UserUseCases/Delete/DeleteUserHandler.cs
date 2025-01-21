using MediatR;
using Project.Application.Models;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.UserUseCases.Delete
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, ResponseModel<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<ResponseModel<string>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(request.id, cancellationToken);

            if (user == null) throw new InvalidOperationException("User not found.");

            user.RequestAccountDeletion();

            _userRepository.Update(user);

            await _unitOfWork.Commit(cancellationToken);

            return new ResponseModel<string>("Your account deletion request has been received. It will be deleted in 30 days.", true);
        }
    }
}
