using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Shared.Behavior;
using Project.Application.Shared.Exceptions;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Application.UseCases.UserUseCases.Create;
using Project.Application.UseCases.UserUseCases.Update;

namespace Project.Application.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationApp(this IServiceCollection services)
        {
            // config automapper, mediatr e fluent validation:
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // config validations in the creation and update layers:
            services.AddTransient<IValidator<UserCreateRequest>, UserValidator<UserCreateRequest>>();
            services.AddTransient<IValidator<UserUpdateRequest>, UserValidator<UserUpdateRequest>>();

            // config Pipeline Behavior:
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // config GlobalExceptionHandler:
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // health application
            services.AddHealthChecks()
            .AddCheck("Application", new ApplicationHealthCheck());
        }
    }
}
