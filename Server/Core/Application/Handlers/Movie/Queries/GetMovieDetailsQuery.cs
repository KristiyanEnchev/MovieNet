namespace Application.Handlers.Movies.Queries
{
    using MediatR;

    using Application.Interfaces;

    using Models.Movie;

    using Domain.Enums;

    using Shared;

    public record GetMovieDetailsQuery : IRequest<Result<MovieDetailsDto>>
    {
        public MediaType MediaType { get; init; }
        public int TmdbId { get; init; }
        public string? UserId { get; init; }
        public bool AppendToResponse { get; init; }

        public GetMovieDetailsQuery(MediaType mediaType, int tmdbId, string? userId = null, bool appendToResponse = false)
        {
            MediaType = mediaType;
            TmdbId = tmdbId;
            UserId = userId;
            AppendToResponse = appendToResponse;
        }

        public class GetMovieDetailsQueryHandler : IRequestHandler<GetMovieDetailsQuery, Result<MovieDetailsDto>>
        {
            private readonly IMovieService _movieService;

            public GetMovieDetailsQueryHandler(IMovieService movieService)
            {
                _movieService = movieService;
            }

            public async Task<Result<MovieDetailsDto>> Handle(GetMovieDetailsQuery request, CancellationToken cancellationToken)
            {
                return await _movieService.GetDetailsAsync(request.MediaType, request.TmdbId, request.AppendToResponse, request.UserId, cancellationToken);
            }
        }
    }
}