namespace Infrastructure.Services.Helpers
{
    using Models.Tmdb;

    using Shared;

    public static class PaginationTransformer
    {
        public static PaginatedResult<T> TransformToPaginatedResult<T>(TmdbPagedResponse<T> tmdbResponse)
        {
            return PaginatedResult<T>.Create(
                data: tmdbResponse.Results,
                count: tmdbResponse.TotalResults,
                pageNumber: tmdbResponse.Page,
                pageSize: tmdbResponse.Results?.Count ?? 0
            );
        }
    }
}