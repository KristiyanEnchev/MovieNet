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

        public async Task<Result<List<GenreDto>>> GetGenresAsync(
            MediaType mediaType,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"genres_{mediaType.ToString().ToLower()}";

            return await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var tmdbResult = await _tmdbService.GetGenresAsync(mediaType, cancellationToken);
                    if (!tmdbResult.Success)
                        return Result<List<GenreDto>>.Failure(tmdbResult.Errors);

                    await SyncGenresAsync(tmdbResult.Data, mediaType, cancellationToken);
                    var genres = await _genreRepository.GetAllGenresAsync(mediaType, cancellationToken);
                    return Result<List<GenreDto>>.SuccessResult(_mapper.Map<List<GenreDto>>(genres));
                },
                forceRefresh ? TimeSpan.Zero : TimeSpan.FromDays(1),
                cancellationToken);
        }

        public async Task<Result<PaginatedResult<MovieDto>>> GetTrendingAsync(
            MediaType mediaType,
            TimeWindow timeWindow,
            bool appendToResponse,
            string userId = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"trending_{mediaType}:{timeWindow}";

            var result = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var tmdbResult = await _tmdbService.FetchTrendingAsync(mediaType, timeWindow, cancellationToken);
                    if (!tmdbResult.Success)
                        return Result<PaginatedResult<MovieDto>>.Failure(tmdbResult.Errors);

                    var movies = _mapper.Map<List<MovieDto>>(tmdbResult.Data.Data);
                    return Result<PaginatedResult<MovieDto>>.SuccessResult(
                        PaginatedResult<MovieDto>.Create(
                            movies,
                            tmdbResult.Data.TotalCount,
                            tmdbResult.Data.CurrentPage,
                            tmdbResult.Data.PageSize));
                },
                TimeSpan.FromMinutes(15),
                cancellationToken);

            if (result.Success && !string.IsNullOrEmpty(userId))
            {
                await EnrichWithUserDataAsync(result.Data.Data, userId, cancellationToken);
            }

            return result;
        }

        public async Task<Result<PaginatedResult<MovieDto>>> SearchAsync(
            MediaType mediatype,
            string query,
            int page = 1,
            string userId = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"search:{mediatype}:{query}:{page}";

            var result = await _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var tmdbResult = await _tmdbService.SearchAsync(query, mediatype, page, cancellationToken);
                    if (!tmdbResult.Success)
                        return Result<PaginatedResult<MovieDto>>.Failure(tmdbResult.Errors);

                    var movies = _mapper.Map<List<MovieDto>>(tmdbResult.Data.Data);
                    return Result<PaginatedResult<MovieDto>>.SuccessResult(
                        PaginatedResult<MovieDto>.Create(
                            movies,
                            tmdbResult.Data.TotalCount,
                            tmdbResult.Data.CurrentPage,
                            tmdbResult.Data.PageSize));
                },
                TimeSpan.FromMinutes(30),
                cancellationToken);

            if (result.Success && !string.IsNullOrEmpty(userId))
            {
                await EnrichWithUserDataAsync(result.Data.Data, userId, cancellationToken);
            }

            return result;
        }

        
    }
}