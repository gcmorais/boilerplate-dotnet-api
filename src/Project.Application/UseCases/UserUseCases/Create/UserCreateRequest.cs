using System.ComponentModel.DataAnnotations;
using MediatR;
using Project.Application.Interfaces;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;

namespace Project.Application.UseCases.UserUseCases.Create
{
    public sealed record UserCreateRequest : IRequest<ResponseModel<UserResponse>>, IUserRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        public string ConfirmPassword { get; set; }
    };
}
