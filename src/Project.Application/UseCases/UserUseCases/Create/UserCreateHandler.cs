using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Project.Application.Interfaces;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.UserUseCases.Create
{
    public class UserCreateHandler : IRequestHandler<UserCreateRequest, ResponseModel<UserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICreateVerifyHash _createVerifyHash;
        //private readonly IEmailService _emailService;
        //private readonly string _confirmationLink;


        public UserCreateHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICreateVerifyHash createVerifyHash
            //IEmailService emailService,
            //IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
            _createVerifyHash = createVerifyHash;
            //_emailService = emailService;
            //_confirmationLink = configuration["Links:ConfirmationLink"];
        }
        public async Task<ResponseModel<UserResponse>> Handle(UserCreateRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmail(request.Email, cancellationToken);
            if (existingUser != null) throw new InvalidOperationException("User with the same email already exists.");

            var existingUsername = await _userRepository.GetByUserName(request.UserName, cancellationToken);
            if (existingUsername != null) throw new InvalidOperationException("User with the same username already exists.");

            // Generate password hash
            _createVerifyHash.CreateHashPassword(request.Password, out byte[] hashPassword, out byte[] saltPassword);

            // Create User
            var user = new User(request.FullName, request.UserName, request.Email, hashPassword, saltPassword);
            user.GenerateEmailConfirmationToken(); // Generate confirmation token

            _userRepository.Create(user);
            await _unitOfWork.Commit(cancellationToken);

            //// Upload and customize the email template
            //var emailTemplatePath = Path.Combine("Infrastructure", "Templates", "Email", "EmailConfirmationTemplate.html");
            //var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath, cancellationToken);

            //var confirmationLink = $"{_confirmationLink}{user.EmailConfirmationToken}";

            //var emailBody = emailTemplate
            //    .Replace("{{UserName}}", user.Fullname)
            //    .Replace("{{ConfirmationLink}}", confirmationLink);

            //// Send the email
            //await _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

            // Return reply
            var userResponse = _mapper.Map<UserResponse>(user);
            return new ResponseModel<UserResponse>(userResponse, true);
        }
    }
}
