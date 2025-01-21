using MediatR;
using Microsoft.Extensions.Configuration;
using Project.Application.Interfaces;
using Project.Application.Models;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, ResponseModel<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly string _passwordResetUrl;

        public ResetPasswordHandler(IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _passwordResetUrl = configuration["Links:PasswordResetUrl"];
        }

        public async Task<ResponseModel<string>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.GeneratePasswordResetToken();
            await _userRepository.Update(user);

            var resetLink = $"{_passwordResetUrl}{user.PasswordResetToken}";

            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", $"Click the link to reset your password: {resetLink}");

            return new ResponseModel<string>("Password reset instructions have been sent to your email.", true);
        }
    }
}
