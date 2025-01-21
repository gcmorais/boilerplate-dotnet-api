using MediatR;
using Project.Application.Models;
using Project.Domain.Interfaces;

namespace Project.Application.UseCases.AuthUseCases.SetNewPassword
{
    public class SetNewPasswordHandler : IRequestHandler<SetNewPasswordRequest, ResponseModel<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreateVerifyHash _createVerifyHash;

        public SetNewPasswordHandler(IUserRepository userRepository, ICreateVerifyHash createVerifyHash)
        {
            _userRepository = userRepository;
            _createVerifyHash = createVerifyHash;
        }

        public async Task<ResponseModel<string>> Handle(SetNewPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByPasswordResetToken(request.Token, cancellationToken);

            if (user == null || !user.ValidatePasswordResetToken(request.Token))
            {
                throw new InvalidOperationException("Invalid or expired token.");
            }

            _createVerifyHash.CreateHashPassword(request.NewPassword, out byte[] newPasswordHash, out byte[] newSalt);

            user.ResetPassword(newPasswordHash, newSalt);
            await _userRepository.Update(user);

            return new ResponseModel<string>("Your password has been successfully updated.", true);
        }
    }
}
