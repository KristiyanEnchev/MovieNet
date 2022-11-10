namespace Application.Handlers.Movies.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Shared;

    using Domain.Enums;

    public record SearchMoviesQuery : IRequest<Result<PaginatedResult<MovieDto>>>
    {
        public MediaType MediaType { get; init; }
        public string Query { get; init; }
        public int Page { get; init; }
        public string? UserId { get; init; }

        public SearchMoviesQuery(MediaType mediaType, string query, int page = 1, string? userId = null)
        {
            Query = query;
            Page = page;
            UserId = userId;
            MediaType = mediaType;
        }

        public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, Result<PaginatedResult<MovieDto>>>
        {
            private readonly IMovieService _movieService;

            public SearchMoviesQueryHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<PaginatedResult<MovieDto>>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
            {
                return await _movieService.SearchAsync(
                    request.MediaType,
                    request.Query,
                    request.Page,
                    request.UserId,
                    cancellationToken);
            }
        }
    }
}