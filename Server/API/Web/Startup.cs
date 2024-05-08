namespace Web
{
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Application;
    using Application.Interfaces;

    using Web.Services;
    using Web.Extensions.Swagger;
    using Web.Extensions.Middleware;
    using Web.Extensions.Healtchecks;

    using Infrastructure;

    using Persistence;
    using Persistence.Context;

    public static class Startup
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly()).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            services.AddApplication(config);
            services.AddInfrastructure(config);
            services.AddPersistence(config);

            services.AddSwaggerDocumentation();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCors();

            services.AddHealth(config);
            services.AddScoped<IUser, CurrentUser>();

            return services;
        }

        public static async Task InitializeDatabase(this IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IWebHostBuilder hostBulder, IWebHostEnvironment env)
        {
            hostBulder.ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.Build();
            });

            AddKestrelConfig(hostBulder);

            return services;
        }

        private static IWebHostBuilder AddKestrelConfig(IWebHostBuilder builder)
        {
            builder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(8080);
            });

            return builder;
        }

        public static IApplicationBuilder UseWeb(this IApplicationBuilder builder)
        {
            builder.UseSwaggerDocumentation()
                    .UseMapper()
                    .UseStaticFiles()
                    .UseHttpsRedirection()
                    .UseErrorHandler()
                    .UseRouting()
                    .UseCors("MovieNet")
                    .UseAuthentication()
                    .UseAuthorization();

            return builder;
        }

        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MovieNet", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "https://mango-hill-028774503-production.westeurope.4.azurestaticapps.net")
                        .WithHeaders("Authorization", "Content-Type")
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapControllers();
            builder.MapHealthCheck();

            return builder;
        }
    }
}