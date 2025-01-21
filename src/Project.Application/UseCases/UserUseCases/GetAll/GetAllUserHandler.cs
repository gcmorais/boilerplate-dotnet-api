using AutoMapper;
using MediatR;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.UserUseCases.GetAll
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserRequest, ResponseModel<List<UserResponse>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUserHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<List<UserResponse>>> Handle(GetAllUserRequest request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAll(cancellationToken);

            if (users == null || !users.Any()) return new ResponseModel<List<UserResponse>>(new List<UserResponse>(), false);

            var userResponse = _mapper.Map<List<UserResponse>>(users);
            return new ResponseModel<List<UserResponse>>(userResponse, true);
        }
    }
}
