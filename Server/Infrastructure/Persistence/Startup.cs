namespace Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Persistence.Context;
    using Domain;
    using Domain.Interfaces;

    public static class Startup
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddRepositories();

            return services;
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(connectionString,
                   builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {

        }
    }
}