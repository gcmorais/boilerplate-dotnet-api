using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Project.Application.UseCases.AdminUseCases.RevokeTokens;

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
