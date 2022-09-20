namespace Application
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using FluentValidation;

    using MediatR;

    using Mapster;

    using Application.Common.Behaviours;

    using Shared.Mappings;

    public static class Startup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMapperConfig();
            services.AddValidators();
            services.AddMediator();
        }

        private static void AddMapperConfig(this IServiceCollection services)
        {
            services.AddMapster();
            MapsterConfig.Configure();
        }

        private static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private static void AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            });
        }
    }
}