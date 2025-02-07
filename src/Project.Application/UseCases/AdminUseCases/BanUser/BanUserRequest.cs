using MediatR;

namespace Project.Application.UseCases.AdminUseCases.BanUser
{
    public class BanUserRequest : IRequest<BanUserResponse>
    {
        public Guid UserId { get; set; }
        public DateTimeOffset? BannedUntil { get; set; }
    }
}
