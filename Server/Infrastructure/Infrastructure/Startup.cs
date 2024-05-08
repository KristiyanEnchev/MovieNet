namespace Infrastructure
{
    using System.Text;
    using System.Net.Http.Headers;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MediatR;

    using StackExchange.Redis;

    using Polly;
    using Polly.Extensions.Http;

    using Persistence.Context;

    using Domain.Entities;

    using Models;

    using Infrastructure.Services.Movie;
    using Infrastructure.Services.Token;
    using Infrastructure.Services.Cache;
    using Infrastructure.BackgroundJobs;
    using Infrastructure.Services.Image;
    using Infrastructure.Services.Identity;

    using Application.Interfaces;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTmdbService(configuration)
                .AddCacheService(configuration)
                .AddBackgroundJobs()
                .AddServices()
                .AddConfigurations(configuration)
                .AddIdentity(configuration)
                .AddCustomAuthentiation(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {

            services
                .AddTransient<IMediator, Mediator>();

            services
                .AddTransient<IMovieService, MovieService>()
                .AddTransient<IUserInteractionService, UserInteractionService>();

            services.AddHttpClient<ImageCacheService>()
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddScoped<IImageCacheService, ImageCacheService>();

            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<IJwtService, JwtService>()
                .AddIdentity<User, UserRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddTokenProvider("MovieNet", typeof(DataProtectorTokenProvider<User>));

            return services;
        }

        public static IServiceCollection AddCustomAuthentiation(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Key))!;
            var audience = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Audience))!;
            var issuer = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Issuer))!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection(nameof(TokenSettings)));

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheOptions>(configuration.GetSection("Cache"));

            var redisConnection = configuration.GetConnectionString("Redis");

            var redisUri = new Uri($"rediss://{redisConnection.Replace("default:", "")}");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{redisUri.Host}:{redisUri.Port},ssl=True,abortConnect=False,password={redisUri.UserInfo}";
                options.InstanceName = configuration["Cache:InstanceName"] ?? "MovieNet_";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { $"{redisUri.Host}:{redisUri.Port}" },
                    Ssl = true,
                    AbortOnConnectFail = false,
                    Password = redisUri.UserInfo
                };

                return ConnectionMultiplexer.Connect(redisOptions);
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddTmdbService(
               this IServiceCollection services,
               IConfiguration configuration)
        {
            services.Configure<TmdbOptions>(
                configuration.GetSection(TmdbOptions.Section));

            services.AddHttpClient<ITmdbService, TmdbService>(client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddHostedService<MovieSyncBackgroundService>();
            return services;
        }
    }
}