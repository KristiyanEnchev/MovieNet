namespace Infrastructure.Services.Movie
{
    using Microsoft.Extensions.Logging;

    using AutoMapper;

    using Application.Interfaces;

    using Persistence.Repositories.Interfaces;

    using Shared;

    using Models.Tmdb;
    using Models.Movie;
    using Models.Tmdb.Enums;

    using Domain.Enums;
    using Domain.Entities;

    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUserMovieInteractionRepository _interactionRepository;
        private readonly ITmdbService _tmdbService;
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;
        private readonly IGenreRepository _genreRepository;

        public MovieService(
            IMovieRepository movieRepository,
            IUserMovieInteractionRepository interactionRepository,
            ITmdbService tmdbService,
            ICacheService cache,
            IMapper mapper,
            ILogger<MovieService> logger,
            IGenreRepository genreRepository)
        {
            _movieRepository = movieRepository;
            _interactionRepository = interactionRepository;
            _tmdbService = tmdbService;
            _cache = cache;
            _mapper = mapper;
            _logger = logger;
            _genreRepository = genreRepository;
        }

        public async Task<Result<MovieDetailsDto>> SyncMovieFromTmdbAsync(
            MediaType mediaType,
            int tmdbId,
            bool appendToResponse,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var tmdbResult = await _tmdbService.GetDetailsAsync(mediaType, tmdbId, appendToResponse, cancellationToken);
                if (!tmdbResult.Success)
                    return Result<MovieDetailsDto>.Failure(tmdbResult.Errors);

                var movie = await _movieRepository.GetByTmdbIdAsync(tmdbId, cancellationToken);

                if (movie == null)
                {
                    movie = new Movie(tmdbId, tmdbResult.Data.Title, tmdbResult.Data.VoteAverage, tmdbResult.Data.BackdropPath, tmdbResult.Data.ReleaseDate);
                    await _movieRepository.AddAsync(movie, cancellationToken);
                }
                else
                {
                    movie.UpdateTmdbData(tmdbResult.Data.TmdbId, tmdbResult.Data.Title);
                    await _movieRepository.UpdateAsync(movie, cancellationToken);
                }

                await _movieRepository.SaveChangesAsync(cancellationToken);
                await InvalidateMovieCacheAsync(movie.Id, mediaType);

                return Result<MovieDetailsDto>.SuccessResult(_mapper.Map<MovieDetailsDto>(tmdbResult.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing {MediaType} from TMDB. TmdbId: {TmdbId}", mediaType, tmdbId);
                return Result<MovieDetailsDto>.Failure($"Error syncing {mediaType}: {ex.Message}");
            }
        }

        private async Task InvalidateMovieCacheAsync(string movieId, MediaType mediaType)
        {
            await _cache.RemoveByPrefixAsync($"{mediaType}:{movieId}:");
        }

        private async Task EnrichWithUserDataAsync(
            MovieDto movie,
            string userId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            var interaction = await _interactionRepository.GetUserInteractionAsync(
                userId,
                movie.TmdbId,
                cancellationToken);

            if (interaction != null)
            {
                movie.IsLiked = interaction.IsLiked;
                movie.IsWatchlisted = interaction.IsWatchlisted;
            }
        }

        
    }
}