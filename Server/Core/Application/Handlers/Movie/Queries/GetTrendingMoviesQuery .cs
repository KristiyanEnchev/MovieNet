namespace Application.Handlers.Movies.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;
    using Models.Tmdb.Enums;

    using Domain.Enums;

    using Shared;

    public record GetTrendingMoviesQuery : IRequest<Result<PaginatedResult<MovieDto>>>
    {
        public MediaType MediaType { get; init; }
        public TimeWindow TimeWindow { get; init; }
        public string? UserId { get; init; }
        public bool AppendToResponse { get; init; }

        public GetTrendingMoviesQuery(MediaType mediaType, TimeWindow timeWindow = TimeWindow.day, string? userId = null, bool appendToResponse = false)
        {
            MediaType = mediaType;
            TimeWindow = timeWindow;
            UserId = userId;
            AppendToResponse = appendToResponse;
        }

        public class GetTrendingMoviesQueryHandler : IRequestHandler<GetTrendingMoviesQuery, Result<PaginatedResult<MovieDto>>>
        {
            private readonly IMovieService _movieService;

            public GetTrendingMoviesQueryHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<PaginatedResult<MovieDto>>> Handle(GetTrendingMoviesQuery request, CancellationToken cancellationToken)
            {
                return await _movieService.GetTrendingAsync(request.MediaType, request.TimeWindow,request.AppendToResponse, request.UserId!, cancellationToken);
            }
        }
    }
}