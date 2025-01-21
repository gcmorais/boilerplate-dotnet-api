using MediatR;
using Project.Application.Models;

namespace Project.Application.UseCases.UserUseCases.Delete
{
    public sealed record DeleteUserRequest(Guid id) : IRequest<ResponseModel<string>>;
}
