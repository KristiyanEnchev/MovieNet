namespace Infrastructure.BackgroundJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Application.Interfaces;

    using Domain.Enums;

    using Models.Tmdb.Enums;

    public class MovieSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MovieSyncBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public MovieSyncBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<MovieSyncBackgroundService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncTrendingMoviesAsync(stoppingToken);

                    var interval = _configuration.GetValue<int>("BackgroundJobs:SyncIntervalMinutes", 60);
                    await Task.Delay(TimeSpan.FromMinutes(interval), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in movie sync background service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task SyncTrendingMoviesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();

                foreach (var mediaType in new[] { MediaType.movie, MediaType.tv })
                {
                    foreach (var timeWindow in new[] { TimeWindow.day, TimeWindow.week })
                    {
                        var result = await movieService.GetTrendingAsync(
                            mediaType,
                            timeWindow,
                            false,
                            userId: null,
                            transformUrls: false,
                            cancellationToken);

                        if (!result.Success)
                        {
                            _logger.LogWarning(
                                "Failed to sync trending {MediaType} for {TimeWindow}: {Errors}",
                                mediaType,
                                timeWindow,
                                string.Join(", ", result.Errors));
                        }
                    }
                }

                _logger.LogInformation("Successfully synced trending movies and TV shows");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing trending media");
                throw;
            }
        }
    }
}