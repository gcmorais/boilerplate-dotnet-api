using MediatR;
using Project.Application.Interfaces;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;

namespace Project.Application.UseCases.UserUseCases.Update
{
    public sealed record UserUpdateRequest : IRequest<ResponseModel<UserResponse>>, IUserRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}
