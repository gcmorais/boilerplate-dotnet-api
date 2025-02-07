using MediatR;

namespace Project.Application.UseCases.AdminUseCases.RevokeTokens
{
    public class RevokeTokensRequest : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
