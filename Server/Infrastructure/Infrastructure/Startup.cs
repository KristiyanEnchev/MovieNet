namespace Infrastructure
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using MediatR;

    using Persistence.Context;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddIdentity(configuration)
                .AddConfigurations(configuration)
                .AddCustomAuthentiation(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IMediator, Mediator>();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddCustomAuthentiation(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}