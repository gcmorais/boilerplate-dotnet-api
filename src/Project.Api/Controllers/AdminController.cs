using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Project.Application.UseCases.AdminUseCases.BanUser;
using Project.Application.UseCases.AdminUseCases.RevokeTokens;
using Project.Application.UseCases.AdminUseCases.UnbanUser;

namespace Project.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HealthCheckService _healthCheckService;

        public AdminController(IMediator mediator, HealthCheckService healthCheckService)
        {
            _mediator = mediator;
            _healthCheckService = healthCheckService;
        }

        [HttpPost("revoke-tokens/{userId}")]
        public async Task<IActionResult> RevokeTokens([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RevokeTokensCommand { UserId = userId }, cancellationToken);
            return NoContent();
        }

        [HttpPost("ban-user")]
        public async Task<IActionResult> BanUser([FromBody] BanUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("unban-user")]
        public async Task<IActionResult> UnbanUser([FromBody] UnbanUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet("healthcheck")]
        public async Task<IActionResult> HealthCheck()
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            if (healthReport.Status == HealthStatus.Healthy)
            {
                return Ok(new { status = "healthy", message = "Application is running smoothly." });
            }

            var errorDetails = healthReport.Entries
                .Where(entry => entry.Value.Status == HealthStatus.Unhealthy)
                .Select(entry => new
                {
                  service = entry.Key,
                  message = entry.Value.Description
                });                          

            return StatusCode(503, new
            {
                status = "unhealthy",
                message = "Application is not responding properly.",
                details = errorDetails
            });
        }
    }
}
