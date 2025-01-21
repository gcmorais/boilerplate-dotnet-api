using FluentValidation;
using Project.Application.Interfaces;

namespace Project.Application.UseCases.UserUseCases.Common
{
    public sealed class UserValidator<T> : AbstractValidator<T> where T : IUserRequest
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).MaximumLength(50);
        }
    }
}
