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

        public async Task<Result<MovieDetailsDto>> GetDetailsAsync(
            MediaType mediaType,
            int tmdbId,
            bool appendToResponse,
            string userId = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{mediaType}_details:{tmdbId}";

            var result = await _cache.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var tmdbResult = await _tmdbService.GetDetailsAsync(mediaType, tmdbId, appendToResponse, cancellationToken);
                    if (!tmdbResult.Success)
                        return Result<MovieDetailsDto>.Failure(tmdbResult.Errors);

                    await SyncMovieFromTmdbAsync(mediaType, tmdbId, appendToResponse, cancellationToken);
                    var movieDto = _mapper.Map<MovieDetailsDto>(tmdbResult.Data);
                    return Result<MovieDetailsDto>.SuccessResult(movieDto);
                },
                TimeSpan.FromHours(1),
                cancellationToken);

            if (result.Success && !string.IsNullOrEmpty(userId))
            {
                await EnrichWithUserDataAsync(result.Data, userId, cancellationToken);
            }

            return result;
        }

        public async Task<Result<PaginatedResult<MovieDto>>> GetAllAsync(
            MediaType mediaType,
            int page = 1,
            SortingOptions sortBy = SortingOptions.popularity_desc,
            int[] withGenres = null,
            string year = null,
            string userId = null,
            CancellationToken cancellationToken = default)
        {
            var genresKey = withGenres != null && withGenres.Any()
                ? string.Join("-", withGenres.OrderBy(g => g))
                : "none";

            var cacheKey = $"discover_{mediaType}:{page}:{sortBy}:{genresKey}:{year ?? "any"}";

            var result = await _cache.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var tmdbResult = await _tmdbService.DiscoverAsync(mediaType, sortBy, withGenres, year, page, cancellationToken);
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

        private async Task EnrichWithUserDataAsync(
             IEnumerable<MovieDto> movies,
             string userId,
             CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId) || !movies.Any())
                return;

            var interactions = await _interactionRepository.GetUserInteractionsAsync(
                userId,
                movies.Select(m => m.TmdbId).ToList(),
                cancellationToken);

            foreach (var movie in movies)
            {
                var interaction = interactions.FirstOrDefault(i => i.MovieId == movie.TmdbId);
                if (interaction != null)
                {
                    movie.IsLiked = interaction.IsLiked;
                    movie.IsDisliked = interaction.IsDisliked;
                    movie.IsWatchlisted = interaction.IsWatchlisted;
                }
            }
        }
        private async Task SyncGenresAsync(
            List<TmdbGenreDto> tmdbGenres,
            MediaType mediaType,
            CancellationToken cancellationToken)
        {
            try
            {
                var genreData = tmdbGenres
                    .Select(g => (g.Id, g.Name, mediaType))
                    .ToList();

                await _genreRepository.GetOrAddGenresAsync(genreData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing genres for {MediaType}", mediaType);
                throw;
            }
        }

        private async Task<List<Genre>> EnrichDbGenres(
          Result<TmdbMovieDetailsDto> tmdbResult,
          MediaType mediaType,
          CancellationToken cancellationToken)
        {
            if (tmdbResult.Data?.Genres == null || !tmdbResult.Data.Genres.Any())
                return new List<Genre>();

            try
            {
                var genreData = tmdbResult.Data.Genres
                    .Select(g => (g.Id, g.Name, mediaType))
                    .ToList();

                return await _genreRepository.GetOrAddGenresAsync(genreData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enriching genres for movie. TmdbId: {TmdbId}", tmdbResult.Data?.TmdbId);
                throw;
            }
        }

        private async Task SyncMoviesAsync(List<int> tmdbIds, MediaType mediaType, bool appendToResponse, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var batch in tmdbIds.Chunk(10))
                {
                    var tasks = batch.Select(id => SyncMovieFromTmdbAsync(mediaType, id, appendToResponse, cancellationToken));
                    await Task.WhenAll(tasks);
                    await Task.Delay(1000, cancellationToken); // Rate limiting between batches
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing {MediaType}s from TMDB", mediaType);
            }
        }
    }
}