using AutoMapper;
using FluentValidation;
using MediatR;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.UserUseCases.Update
{
    public class UserUpdateHandler : IRequestHandler<UserUpdateRequest, ResponseModel<UserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UserUpdateRequest> _validator;

        public UserUpdateHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IMapper mapper, IValidator<UserUpdateRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ResponseModel<UserResponse>> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.GetById(request.Id, cancellationToken);

            if (user == null) throw new InvalidOperationException("User not found.");

            if (!string.IsNullOrWhiteSpace(request.FullName)) user.UpdateName(request.FullName);

            if (!string.IsNullOrWhiteSpace(request.UserName)) user.UpdateUsername(request.UserName);

            if (!string.IsNullOrWhiteSpace(request.Email)) user.UpdateEmail(request.Email);

            _userRepository.Update(user);

            await _unitOfWork.Commit(cancellationToken);

            var userResponse = _mapper.Map<UserResponse>(user);

            return new ResponseModel<UserResponse>(userResponse, true);
        }
    }
}
