using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.Application.UseCases.AdminUseCases.RevokeTokens;

namespace Project.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("revoke-tokens/{userId}")]
        public async Task<IActionResult> RevokeTokens([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RevokeTokensCommand { UserId = userId }, cancellationToken);
            return NoContent();
        }
    }
}
