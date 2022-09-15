namespace Persistence
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Domain;
    using Domain.Entities;
    using Domain.Interfaces;

    using Persistence.Context;

    public static class Startup
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ApplicationDbInitializer>();

            services.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }
    }
}