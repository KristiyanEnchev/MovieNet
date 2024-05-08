namespace Application.Interfaces
{
    using Domain.Enums;

    using Models.Movie;
    using Models.Tmdb.Enums;

    using Shared;

    public interface IMovieService
    {
        Task<Result<PaginatedResult<MovieDto>>> GetTrendingAsync(
           MediaType mediaType,
           TimeWindow timeWindow,
           bool appendToResponse,
           string userId = null,
           bool transformUrls = true,
           CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<MovieDto>>> SearchAsync(
            MediaType mediaType,
            string query,
            int page = 1,
            string userId = null,
            CancellationToken cancellationToken = default);

        Task<Result<MovieDetailsDto>> GetDetailsAsync(
            MediaType mediaType,
            int tmdbId,
            bool appendToResponse,
            string userId = null,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<MovieDto>>> GetAllAsync(
            MediaType mediaType,
            int page = 1,
            SortingOptions sortBy = SortingOptions.popularity_desc,
            int[] withGenres = null,
            string year = null,
            string userId = null,
            CancellationToken cancellationToken = default);

        Task<Result<List<GenreDto>>> GetGenresAsync(
            MediaType mediaType,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default);

        Task<Result<MovieDetailsDto>> SyncMovieFromTmdbAsync(
            MediaType mediaType,
            int tmdbId,
            bool appendToResponse,
            CancellationToken cancellationToken = default);
    }
}