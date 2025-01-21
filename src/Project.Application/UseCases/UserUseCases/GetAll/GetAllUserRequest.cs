using MediatR;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;

namespace Project.Application.UseCases.UserUseCases.GetAll
{
    public sealed record GetAllUserRequest : IRequest<ResponseModel<List<UserResponse>>>;
}
