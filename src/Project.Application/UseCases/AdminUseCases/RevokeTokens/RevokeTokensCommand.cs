using MediatR;

namespace Project.Application.UseCases.AdminUseCases.RevokeTokens
{
    public class RevokeTokensCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
