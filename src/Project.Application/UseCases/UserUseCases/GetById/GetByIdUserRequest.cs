using MediatR;
using Project.Application.Models;
using Project.Application.UseCases.UserUseCases.Common;

namespace Project.Application.UseCases.UserUseCases.GetById
{
    public sealed record GetByIdUserRequest(Guid id) : IRequest<ResponseModel<UserResponse>>;
}
