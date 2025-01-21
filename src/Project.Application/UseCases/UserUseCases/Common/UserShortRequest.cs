using MediatR;

namespace Project.Application.UseCases.UserUseCases.Common
{
    public sealed record UserShortRequest(Guid Id, string Email, string FullName) : IRequest<UserShortResponse>;
}
