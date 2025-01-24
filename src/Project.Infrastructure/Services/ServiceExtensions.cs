using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Interfaces;
using Project.Application.Services;
using Project.Domain.Interfaces;
using Project.Infrastructure.Context;
using Project.Infrastructure.Repositories;

namespace Project.Infrastructure.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICreateVerifyHash, ServiceHash>();
            services.AddScoped<ServiceHash>();
            services.AddScoped<AccountDeletionService>();
            services.AddHostedService<AccountDeletionHostedService>();

            // SendGrid config
            services.AddSingleton<IEmailService>(sp =>
            {
                var sendGridApiKey = configuration["SendGrid:ApiKey"];
                if (string.IsNullOrEmpty(sendGridApiKey))
                {
                    throw new InvalidOperationException("SendGrid API Key not configured in appsettings.json.");
                }
                return new EmailService(sendGridApiKey);
            });

        }
    }
}
