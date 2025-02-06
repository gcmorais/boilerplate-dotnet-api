using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Project.Application.Services
{
    public class ApplicationHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isAppHealthy = true;
            if (!isAppHealthy)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Application is experiencing issues"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Application is running smoothly"));
        }
    }

}
