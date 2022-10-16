namespace Application
{
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using FluentValidation;

    using MediatR;

    using AutoMapper;

    using Application.Common.Mappings;
    using Application.Common.Behaviours;

    public static class Startup
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapperConfig(configuration);
            services.AddValidators();
            services.AddMediator();
        }

        private static void AddAutoMapperConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationAssembly = Assembly.GetExecutingAssembly();
            var modelAssembly = Assembly.GetAssembly(typeof(Models.TokenSettings));

            //services.AddAutoMapper(config =>
            //{
            //    config.AllowNullCollections = true;
            //    config.AllowNullDestinationValues = true;

            //}, new[] { applicationAssembly });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.AddProfile<MovieMappingProfile>();
            });

            services.AddSingleton(config.CreateMapper());

            services.AddTransient<AutoMapperConfigurationValidator>();
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

        public static IApplicationBuilder UseMapper(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AutoMapperConfigurationValidator>();
            }

            return builder;
        }
    }
}