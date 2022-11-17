namespace Application.Handlers.Movies.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;
    using Models.Tmdb.Enums;

    using Domain.Enums;

    using Shared;

    public record GetAllMoviesQuery : IRequest<Result<PaginatedResult<MovieDto>>>
    {
        public MediaType MediaType { get; init; }
        public int Page { get; init; }
        public SortingOptions SortBy { get; init; }
        public string? WithGenres { get; init; }
        public string? Year { get; init; }
        public string? UserId { get; init; }

        public GetAllMoviesQuery(
            MediaType mediaType,
            string? withGenres,
            int page = 1,
            string? userId = null,
            SortingOptions sortBy = SortingOptions.popularity_desc,
            string? year = null)
        {
            MediaType = mediaType;
            Page = page;
            UserId = userId;
            SortBy = sortBy;
            WithGenres = withGenres;
            Year = year;
        }

        public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, Result<PaginatedResult<MovieDto>>>
        {
            private readonly IMovieService _movieService;

            public GetAllMoviesQueryHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<PaginatedResult<MovieDto>>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
            {
                int[]? genreIds = null;
                if (!string.IsNullOrEmpty(request.WithGenres))
                {
                    genreIds = request.WithGenres.Split(',')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => int.TryParse(s.Trim(), out int id) ? id : -1)
                        .Where(id => id != -1)
                        .ToArray();
                }

                return await _movieService.GetAllAsync(
                    request.MediaType,
                    request.Page,
                    request.SortBy,
                    genreIds,
                    request.Year,
                    request.UserId,
                    cancellationToken);
            }
        }
    }
}