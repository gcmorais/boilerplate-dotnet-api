using AutoMapper;
using MediatR;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.UserUseCases.GetById
{
    public class GetByIdUserHandler : IRequestHandler<GetByIdUserRequest, ResponseModel<UserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetByIdUserHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<UserResponse>> Handle(GetByIdUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(request.id, cancellationToken);

            if (user == null) throw new KeyNotFoundException($"User with ID {request.id} not found.");

            var userResponse = _mapper.Map<UserResponse>(user);

            return new ResponseModel<UserResponse>(userResponse, true);
        }
    }
}
