namespace Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Persistence.Context;
    using Persistence.Repositories;
    using Persistence.Repositories.Interfaces;

    using Domain;
    using Domain.Interfaces;

    using Shared.Interfaces;

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
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });
            });

            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITransactionHelper, TransactionHelper>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserMovieInteractionRepository, UserMovieInteractionRepository>();
        }
    }
}