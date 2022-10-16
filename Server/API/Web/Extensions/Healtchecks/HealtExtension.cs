namespace Web.Extensions.Healtchecks
{
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;

    using Domain.Enums;

    using Application.Interfaces;

    using Models.HealthCheck;
    using Models.Tmdb.Enums;

    internal static class HealtExtension
    {
        internal static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
        {
            var healthSettings = configuration.GetSection(nameof(Health)).Get<Health>();
            services.AddSingleton<CustomHealthCheckResponseWriter>();

            var healthChecks = services.AddHealthChecks();

            // Database health check
            if (healthSettings?.DatabaseHealthChecks == true)
            {
                healthChecks.AddNpgSql(
                    configuration.GetConnectionString("DefaultConnection")!,
                    name: "database",
                    tags: new[] { "database", "postgres" });
            }

            // Redis health check
            healthChecks.AddCheck<RedisHealthCheck>(
                "redis_cache",
                tags: new[] { "cache", "redis" });

            // TMDB API health check
            healthChecks.AddCheck<TmdbHealthCheck>(
                "tmdb_api",
                tags: new[] { "api", "external" });

            // Movie service health check
            healthChecks.AddCheck<MovieServiceHealthCheck>(
                "movie_service",
                tags: new[] { "service", "internal" });

            // Memory health check
            healthChecks.AddCheck(
                "memory",
                new MemoryHealthCheck(1024L * 1024L * 1024L),
                tags: new[] { "memory", "system" });

            return services;
        }

        public class ControllerHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                bool isControllerHealthy = true;

                if (isControllerHealthy)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Controller is healthy"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy("Controller is unhealthy"));
            }
        }

        public class CacheHealthCheck : IHealthCheck
        {
            private readonly IMemoryCache _cache;
            private readonly string _testCacheKey = "health_check_cache_key";
            private readonly string _testCacheValue = "test_value";

            public CacheHealthCheck(IMemoryCache cache)
            {
                _cache = cache;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    _cache.Set(_testCacheKey, _testCacheValue);

                    if (_cache.TryGetValue(_testCacheKey, out string cachedValue))
                    {
                        if (cachedValue == _testCacheValue)
                        {
                            return Task.FromResult(HealthCheckResult.Healthy("Cache is healthy"));
                        }
                    }

                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache is not working as expected"));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache health check failed", ex));
                }
            }
        }

        public class TmdbHealthCheck : IHealthCheck
        {
            private readonly ITmdbService _tmdbService;
            private readonly ILogger<TmdbHealthCheck> _logger;

            public TmdbHealthCheck(ITmdbService tmdbService, ILogger<TmdbHealthCheck> logger)
            {
                _tmdbService = tmdbService;
                _logger = logger;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(
                HealthCheckContext context,
                CancellationToken cancellationToken = default)
            {
                try
                {
                    var result = await _tmdbService.FetchTrendingAsync(
                        MediaType.movie,
                        TimeWindow.day,
                        cancellationToken);

                    return result.Success
                        ? HealthCheckResult.Healthy("TMDB API is responding normally")
                        : HealthCheckResult.Degraded("TMDB API response indicates issues");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TMDB API health check failed");
                    return HealthCheckResult.Unhealthy("TMDB API is not responding", ex);
                }
            }
        }

        public class MemoryHealthCheck : IHealthCheck
        {
            private readonly long _maxAllowedMemory;

            public MemoryHealthCheck(long maxAllowedMemory)
            {
                _maxAllowedMemory = maxAllowedMemory;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var memoryUsed = GC.GetTotalMemory(false);

                if (memoryUsed <= _maxAllowedMemory)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Memory usage is within limits"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high. Current usage: {memoryUsed / 1024 / 1024} MB"));
            }
        }

        public class RedisHealthCheck : IHealthCheck
        {
            private readonly ICacheService _cache;
            private readonly ILogger<RedisHealthCheck> _logger;
            private const string TestKey = "health_check_test";

            public RedisHealthCheck(ICacheService cache, ILogger<RedisHealthCheck> logger)
            {
                _cache = cache;
                _logger = logger;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(
                HealthCheckContext context,
                CancellationToken cancellationToken = default)
            {
                try
                {
                    await _cache.SetAsync(TestKey, "test_value", TimeSpan.FromSeconds(1), cancellationToken);
                    var result = await _cache.GetAsync<string>(TestKey, cancellationToken);

                    return result == "test_value"
                        ? HealthCheckResult.Healthy("Redis cache is working properly")
                        : HealthCheckResult.Degraded("Redis cache read/write test failed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Redis health check failed");
                    return HealthCheckResult.Unhealthy("Redis cache is not responding", ex);
                }
            }
        }

        public class MovieServiceHealthCheck : IHealthCheck
        {
            private readonly IMovieService _movieService;
            private readonly ILogger<MovieServiceHealthCheck> _logger;

            public MovieServiceHealthCheck(IMovieService movieService, ILogger<MovieServiceHealthCheck> logger)
            {
                _movieService = movieService;
                _logger = logger;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(
                HealthCheckContext context,
                CancellationToken cancellationToken = default)
            {
                try
                {
                    var result = await _movieService.GetTrendingAsync(
                        MediaType.movie,
                        TimeWindow.day,
                        false,
                        null,
                        cancellationToken);

                    if (!result.Success)
                    {
                        return HealthCheckResult.Degraded("Movie service returned unsuccessful response");
                    }

                    var hasData = result.Data?.Data?.Any() == true;
                    return hasData
                        ? HealthCheckResult.Healthy("Movie service is functioning normally")
                        : HealthCheckResult.Degraded("Movie service returned no data");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Movie service health check failed");
                    return HealthCheckResult.Unhealthy("Movie service check failed", ex);
                }
            }
        }

        internal static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = (httpContext, result) => CustomHealthCheckResponseWriter.WriteResponse(httpContext, result),
            });
    }
}