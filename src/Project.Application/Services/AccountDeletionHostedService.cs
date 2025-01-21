using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Project.Application.Services
{
    public class AccountDeletionHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AccountDeletionHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var deletionService = scope.ServiceProvider.GetRequiredService<AccountDeletionService>();
                    await deletionService.DeleteMarkedAccountsAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
