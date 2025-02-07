using MediatR;

namespace Project.Application.UseCases.AdminUseCases.UnbanUser
{
    public class UnbanUserRequest : IRequest<UnbanUserResponse>
    {
        public Guid UserId { get; set; }
    }
}
