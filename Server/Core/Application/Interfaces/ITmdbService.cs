namespace Application.Interfaces
{
    using Shared;

    using Models.Tmdb;
    using Models.Tmdb.Enums;

    using Domain.Enums;

    public interface ITmdbService
    {
        Task<Result<PaginatedResult<TmdbMovieDto>>> FetchTrendingAsync(
             MediaType mediaType,
             TimeWindow timeWindow = TimeWindow.day,
             CancellationToken cancellationToken = default);

        Task<Result<TmdbMovieDetailsDto>> GetDetailsAsync(
            MediaType mediaType,
            int tmdbId,
            bool appendToResponse,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<TmdbMovieDto>>> SearchAsync(
            string query,
            MediaType mediatype,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<TmdbMovieDto>>> DiscoverAsync(
            MediaType mediaType = MediaType.movie,
            SortingOptions sortBy = SortingOptions.popularity_desc,
            int[] withGenres = null,
            string year = null,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<Result<List<TmdbGenreDto>>> GetGenresAsync(
            MediaType mediaType,
            CancellationToken cancellationToken = default);

        Task<Result<TmdbCreditsDto>> GetCreditsAsync(
            MediaType mediaType,
            int tmdbId,
            CancellationToken cancellationToken = default);

        Task<Result<TmdbVideoResultsDto>> GetVideosAsync(
            MediaType mediaType,
            int tmdbId,
            CancellationToken cancellationToken = default);
    }
}
