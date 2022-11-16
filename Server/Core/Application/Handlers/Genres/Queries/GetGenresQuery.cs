namespace Application.Handlers.Genres.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Domain.Enums;

    using Shared;

    public record GetGenresQuery : IRequest<Result<List<GenreDto>>>
    {
        public MediaType MediaType { get; init; }
        public bool ForceRefresh { get; init; }

        public GetGenresQuery(MediaType mediaType, bool forceRefresh = false)
        {
            MediaType = mediaType;
            ForceRefresh = forceRefresh;
        }

        public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, Result<List<GenreDto>>>
        {
            private readonly IMovieService _movieService;

            public GetGenresQueryHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<List<GenreDto>>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
            {
                return await _movieService.GetGenresAsync(request.MediaType, request.ForceRefresh, cancellationToken);
            }
        }
    }
}